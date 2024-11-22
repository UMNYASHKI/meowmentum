package service

import (
	"crypto/sha1"
	"errors"
	"fmt"
	"github.com/joho/godotenv"
	"gopkg.in/yaml.v3"
	"log"
	"os"
	"os/exec"
	"os/signal"
	"slices"
	"sort"
	"strconv"
	"strings"
	"time"
)

type Registry struct {
	Services             map[string]*Service
	SortedKeys           []string
	OpenPortReservations map[int]string
	Excludes             []string
}

func LoadServiceRegistry(excludes []string, exposeAll bool, genFn func(*Registry) error) (*Registry, error) {
	reg := &Registry{
		Services:             make(map[string]*Service),
		SortedKeys:           make([]string, 0),
		OpenPortReservations: make(map[int]string),
		Excludes:             excludes,
	}

	data, err := ParseServiceFilesInProject()
	if err != nil {
		return nil, err
	}

	excludesFound := make([]string, 0)

	for fileName, services := range data {
		for serviceName, yamlEntry := range services {
			if _, ok := reg.Services[serviceName]; ok {
				return nil, fmt.Errorf("duplicate service name: %s", serviceName)
			}

			isExcluded := false

			if slices.Contains(excludes, serviceName) {
				isExcluded = true
				excludesFound = append(excludesFound, serviceName)
			}

			service, err := FromYamlEntry(fileName, serviceName, yamlEntry, isExcluded)
			if err != nil {
				return nil, err
			}

			for _, exp := range service.Expose {
				if exp.IsExposed {
					if _, ok := reg.OpenPortReservations[exp.OuterPort]; ok {
						return nil, fmt.Errorf(
							"port %d reservation conflict: %s and %s",
							exp.OuterPort,
							reg.OpenPortReservations[exp.OuterPort],
							serviceName,
						)
					}
					reg.OpenPortReservations[exp.OuterPort] = serviceName
				}
			}

			reg.Services[serviceName] = service
			reg.SortedKeys = append(reg.SortedKeys, serviceName)
		}
	}

	if len(excludesFound) != len(excludes) {
		notFound := make([]string, 0)
		for _, exclude := range excludes {
			if !slices.Contains(excludesFound, exclude) {
				notFound = append(notFound, exclude)
			}
		}

		return nil, fmt.Errorf("excluded services not found: %v", notFound)
	}

	sort.Strings(reg.SortedKeys)

	for _, service := range reg.Services {
		for name, exp := range service.Expose {
			if !exp.IsExposed {
				port := calcOpenPort(fmt.Sprintf("%s:%s", service.Name, name))
				for _, ok := reg.OpenPortReservations[port]; ok; {
					port++
					if port > rangeEnd {
						port = rangeStart
					}
				}

				exp.OuterPort = port
				reg.OpenPortReservations[port] = service.Name

				if exposeAll {
					exp.IsExposed = true
				}
			}
		}
	}

	for _, service := range reg.Services {
		for _, attach := range service.Attach {
			target, ok := reg.Services[attach.AttachService]
			if !ok {
				return nil, fmt.Errorf("service %s not found", attach.AttachService)
			}

			targetExpose, ok := target.Expose[attach.AttachTo]
			if !ok {
				return nil, fmt.Errorf("service %s does not expose %s", attach.AttachService, attach.AttachTo)
			}

			attach.ResolvedAttachService = target
			attach.ResolvedAttachTo = targetExpose

			if service.Excluded {
				targetExpose.IsExposed = true
			}
		}
	}

	err = genFn(reg)
	if err != nil {
		return nil, fmt.Errorf("failed to run generators: %v", err)
	}

	return reg, nil
}

var rangeStart = 55000
var rangeEnd = 56000

func calcOpenPort(serverIdent string) int {
	sha := sha1.New()
	sha.Write([]byte(serverIdent))
	hash := sha.Sum(nil)
	sum := 0
	sum |= int(hash[0])
	sum |= int(hash[1]) << 8
	sum |= int(hash[2]) << 16
	sum |= int(hash[3]) << 24
	return rangeStart + (sum % (rangeEnd - rangeStart))
}

func (r *Registry) WriteDockerCompose() error {
	root := map[string]interface{}{
		"name": fmt.Sprintf("meowmentum-%s", os.Getenv("ENVIRONMENT")),
	}

	services := make(map[string]interface{})
	root["services"] = services

	volumes := make(map[string]interface{})
	root["volumes"] = volumes

	for _, key := range r.SortedKeys {
		service := r.Services[key]

		if service.Excluded {
			continue
		}

		srv := map[string]interface{}{
			"extra_hosts": []string{
				"host.docker.internal:host-gateway",
			},
		}

		services[service.GetComposeServiceName()] = srv

		srv["image"] = service.Image
		srv["container_name"] = service.GetComposeServiceName()
		srv["restart"] = "unless-stopped"

		if service.Build != nil {
			build := make(map[string]interface{})
			srv["build"] = build

			build["context"] = service.Build.Context

			if service.Build.Dockerfile != nil {
				build["dockerfile"] = service.Build.Dockerfile
			}

			if len(service.Build.Args) > 0 {
				build["args"] = service.Build.Args
			}

			if len(service.Build.AdditionalContexts) > 0 {
				build["additional_contexts"] = service.Build.AdditionalContexts
			}

			if service.Build.Target != nil {
				build["target"] = service.Build.Target
			}
		}

		vlms := make([]string, 0)

		for target, volume := range service.Volumes {
			vlms = append(vlms, volume.String()+":"+target)

			if v, ok := volume.(*NameVolume); ok {
				if _, ok := volumes[v.Name]; ok {
					log.Printf("warning: volume %s config defined twise, it's up to you to keep it in sync", v.Name)
				}

				if v.VolumeConfig != nil {
					volumes[v.Name] = v.VolumeConfig
				}
			}
		}

		if len(vlms) > 0 {
			srv["volumes"] = vlms
		}

		if len(service.Labels) > 0 {
			srv["labels"] = service.Labels
		}

		if service.Yaml.ComposeOverrides != nil {
			for k, v := range service.Yaml.ComposeOverrides {
				srv[k] = &v
			}
		}

		environment := make(map[string]string)
		ports := make([]string, 0)

		environment["ENVIRONMENT"] = os.Getenv("ENVIRONMENT")

		for name, exp := range service.Expose {
			if exp.IsExposed {
				ports = append(ports, fmt.Sprintf("${ENVIRONMENT_BIND_ADDR}:%d:%d", exp.OuterPort, exp.InnerPort))
			}

			environment[exp.Type.GetExposureVar(name)] = fmt.Sprintf("127.0.0.1:%d", exp.InnerPort)
		}

		if len(ports) > 0 {
			srv["ports"] = ports
		}

		for _, att := range service.Attach {
			if att.ResolvedAttachTo.Type.CanBeAttached() {
				environment[att.ResolvedAttachTo.Type.GetAttachmentVar(att.ResolvedAttachTo.Name)] = att.ResolvedAttachTo.GetComposeServiceAddress()
			}
		}

		for k, value := range envMapPreEval(service.EnvMap, environment) {
			environment[k] = value
		}

		for _, value := range service.EnvPass {
			environment[value] = fmt.Sprintf("${%s}", value)
		}

		if len(environment) > 0 {
			srv["environment"] = environment
		}
	}

	content, err := yaml.Marshal(root)
	if err != nil {
		return err
	}

	err = os.WriteFile(fmt.Sprintf("./build/%s.docker-compose.yml", os.Getenv("ENVIRONMENT")), content, 0644)
	if err != nil {
		return err
	}

	return nil
}

func envMapPreEval(envMap map[string]string, envContext map[string]string) map[string]string {
	result := make(map[string]string)
	for k, v := range envMap {
		for ek, ev := range envContext {
			v = strings.ReplaceAll(v, fmt.Sprintf("${%s}", ek), ev)
			v = strings.ReplaceAll(v, fmt.Sprintf("$%s", ek), ev)
		}
		result[k] = v
	}
	return result
}

func (r *Registry) RunApp() {
	if os.Getenv("ENVIRONMENT_BIND_ADDR") == "" {
		_ = os.Setenv("ENVIRONMENT_BIND_ADDR", "127.0.0.1")
	}

	err := godotenv.Load(".env")
	if err != nil {
		log.Fatal("You need to have .env file in the project root, create it from .env.example")
	}

	{
		maxLenA := 0
		maxLenB := 0

		exposes := make([]string, 0)
		exposeAddrs := make(map[string]string)

		for _, key := range r.SortedKeys {
			service := r.Services[key]
			if service.Excluded {
				continue
			}

			for name, exp := range service.Expose {
				if exp.IsExposed {
					exposeId := fmt.Sprintf("%s:%s", service.Name, name)
					exposeAddr := fmt.Sprintf("%s:%d", os.Getenv("ENVIRONMENT_BIND_ADDR"), exp.OuterPort)
					exposes = append(exposes, exposeId)
					exposeAddrs[exposeId] = exposeAddr

					if len(exposeId) > maxLenA {
						maxLenA = len(exposeId)
					}
					if len(exposeAddr) > maxLenB {
						maxLenB = len(exposeAddr)
					}
				}
			}
		}

		if len(exposes) > 0 {
			fmt.Println("[ EXPOSED SERVICES ]")

			if maxLenA < 15 {
				maxLenA = 15
			}

			if maxLenB < 7 {
				maxLenB = 7
			}

			fmt.Printf("| %-*s | %-*s |\n", maxLenA, "Expose", maxLenB, "Address")
			fmt.Printf("| %s + %s |\n", strings.Repeat("-", maxLenA), strings.Repeat("-", maxLenB))

			sort.Strings(exposes)
			for _, expose := range exposes {
				fmt.Printf("| %-*s | %s |\n", maxLenA, expose, exposeAddrs[expose])
			}

			fmt.Println()
		}

		for _, key := range r.SortedKeys {
			service := r.Services[key]
			if service.Excluded {
				envLines := make([]string, 0)
				environment := make(map[string]string)

				envLines = append(envLines, fmt.Sprintf("ENVIRONMENT=%s", os.Getenv("ENVIRONMENT")))
				environment["ENVIRONMENT"] = os.Getenv("ENVIRONMENT")

				for _, exp := range service.Expose {
					envLines = append(envLines, fmt.Sprintf("%s=127.0.0.1:%d", exp.Type.GetExposureVar(exp.Name), exp.OuterPort))
					environment[exp.Type.GetExposureVar(exp.Name)] = fmt.Sprintf("127.0.0.1:%d", exp.OuterPort)
				}

				for _, att := range service.Attach {
					if att.ResolvedAttachTo.Type.CanBeAttached() {
						envLines = append(envLines,
							fmt.Sprintf("%s=%s",
								att.ResolvedAttachTo.Type.GetAttachmentVar(att.ResolvedAttachTo.Name),
								att.ResolvedAttachTo.GetComposeServiceAddress(),
							),
						)
						environment[att.ResolvedAttachTo.Type.GetAttachmentVar(att.ResolvedAttachTo.Name)] = att.ResolvedAttachTo.GetComposeServiceAddress()
					}
				}

				for k, value := range envMapPreEval(service.EnvMap, environment) {
					v := os.ExpandEnv(value)
					if strings.Contains(v, " ") {
						v = strconv.Quote(v)
					}
					envLines = append(envLines, fmt.Sprintf("%s=%s", k, v))
				}

				for _, value := range service.EnvPass {
					v := os.Getenv(value)
					if strings.Contains(v, " ") {
						v = strconv.Quote(v)
					}
					envLines = append(envLines, fmt.Sprintf("%s=%s", value, v))
				}

				if len(envLines) > 0 {
					fmt.Printf("[ EXTERNAL %s ENVIRONMENT ]\n", service.Name)
					for _, line := range envLines {
						fmt.Println(line)
					}
					fmt.Println()
				}
			}
		}
	}

	fmt.Printf("Press Enter to START!")
	_, _ = fmt.Scanln()

	args := []string{
		"compose", "-f", fmt.Sprintf("./build/%s.docker-compose.yml", os.Getenv("ENVIRONMENT")), "up",
		"--build", "--renew-anon-volumes",
	}

	if len(r.Excludes) == 0 {
		args = append(args, "--remove-orphans")
	}

	cmd := exec.Command("docker", args...)
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	cmd.Stdin = os.Stdin

	if err := cmd.Start(); err != nil {
		log.Fatalf("Failed to start command: %v", err)
	}

	signals := make(chan os.Signal, 1)
	signal.Notify(signals)

	go func() {
		for sig := range signals {
			if cmd.Process != nil {
				_ = cmd.Process.Signal(sig)
			}
		}
	}()

	err = cmd.Wait()
	exitCode := 0
	if err != nil {
		var exitError *exec.ExitError
		if errors.As(err, &exitError) {
			exitCode = exitError.ExitCode()
		} else {
			log.Printf("Command execution failed: %v", err)
		}
	}

	time.Sleep(100 * time.Millisecond)
	os.Exit(exitCode)
}

func (r *Registry) BuildApp() {
	log.Println("Building images...")
	cmd := exec.Command("docker", "compose", "-f", fmt.Sprintf("./build/%s.docker-compose.yml", os.Getenv("ENVIRONMENT")), "build")
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	cmd.Stdin = os.Stdin

	if err := cmd.Start(); err != nil {
		log.Fatalf("Failed to start command: %v", err)
	}

	signals := make(chan os.Signal, 1)
	signal.Notify(signals)

	go func() {
		for sig := range signals {
			if cmd.Process != nil {
				_ = cmd.Process.Signal(sig)
			}
		}
	}()

	err := cmd.Wait()
	exitCode := 0
	if err != nil {
		var exitError *exec.ExitError
		if errors.As(err, &exitError) {
			exitCode = exitError.ExitCode()
		} else {
			log.Printf("Command execution failed: %v", err)
		}
	}

	if exitCode != 0 {
		log.Printf("Command execution failed with exit code: %d", exitCode)
		time.Sleep(100 * time.Millisecond)
		os.Exit(exitCode)
	}

	log.Println("Images built successfully!")
}

func (r *Registry) SaveImages() {
	log.Println("Saving images...")
	_ = os.MkdirAll("./build/images", os.ModePerm)

	args := []string{
		"save", "-o",
		"./build/images/" + fmt.Sprintf("%s-%s-images.tar", os.Getenv("ENVIRONMENT"), os.Getenv("GIT_COMMIT")),
	}

	for _, key := range r.SortedKeys {
		service := r.Services[key]
		if service.Excluded {
			continue
		}

		args = append(args, service.Image)
	}

	cmd := exec.Command("docker", args...)
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	cmd.Stdin = os.Stdin

	if err := cmd.Start(); err != nil {
		log.Fatalf("Failed to start command: %v", err)
	}

	signals := make(chan os.Signal, 1)
	signal.Notify(signals)

	go func() {
		for sig := range signals {
			if cmd.Process != nil {
				_ = cmd.Process.Signal(sig)
			}
		}
	}()

	err := cmd.Wait()
	exitCode := 0
	if err != nil {
		var exitError *exec.ExitError
		if errors.As(err, &exitError) {
			exitCode = exitError.ExitCode()
		} else {
			log.Printf("Command execution failed: %v", err)
		}
	}

	if exitCode != 0 {
		log.Printf("Command execution failed with exit code: %d", exitCode)
		time.Sleep(100 * time.Millisecond)
		os.Exit(exitCode)
	}

	log.Println("Images saved successfully!")
}
