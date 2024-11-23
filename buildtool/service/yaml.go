package service

import (
	"fmt"
	"gopkg.in/yaml.v3"
	"log"
	"os"
	"path/filepath"
	"regexp"
	"strings"
)

var regexExposeName = regexp.MustCompile("^[a-z0-9-]+$")

type YamlEntry struct { // NOTE: paths are relative to the current services.yml file, or to repo root if they begin with "/"
	Image string    `yaml:"image"` // Name for docker image, required
	Build *struct { // Build is optional
		Context            string            `yaml:"context"`              // If build is set, context is required
		Dockerfile         *string           `yaml:"dockerfile,omitempty"` // Optional path to Dockerfile, relative to services.yml, not context
		Args               map[string]string `yaml:"args,omitempty"`
		AdditionalContexts map[string]string `yaml:"additional_contexts,omitempty"`
		Target             *string           `yaml:"target,omitempty"` // Optional, specifies stage where runner is built
	} `yaml:"build,omitempty"`
	Volumes map[string]struct { // Path or Name must be set, but not both
		Path   *string   `yaml:"path,omitempty"`
		Name   *string   `yaml:"name,omitempty"`
		Config yaml.Node `yaml:"config,omitempty"`
	} `yaml:"volumes,omitempty"`
	Labels           map[string]string    `yaml:"labels,omitempty"`            // Optional, map of labels to set on the container
	ComposeOverrides map[string]yaml.Node `yaml:"compose_overrides,omitempty"` // Optional, map of compose overrides
	Expose           map[string]struct {
		Type   string            `yaml:"type"`             // Can be "static", "grpc", "http"
		Port   *int              `yaml:"port,omitempty"`   // Required for "static", optional for others. Port inside container
		Routes map[string]string `yaml:"routes,omitempty"` // Used only for "http"
		Pin    *int              `yaml:"pin,omitempty"`    // Optional, sets fixed outside port and force exposes service
	} `yaml:"expose,omitempty"`
	Attach            []string          `yaml:"attach,omitempty"`              // List of service exposures to attach to, in format "service:exposure"
	EnvMap            map[string]string `yaml:"envmap,omitempty"`              // Map of env variables to set. Evaluated only at runtime (unlike other fields here)
	EnvPass           []string          `yaml:"envpass,omitempty"`             // Environment variable names to pass from host, used for secrets and cfg
	SplitAddressParts bool              `yaml:"split_address_parts,omitempty"` // Add host and port parts separately to env for attach and expose
}

func (y *YamlEntry) Validate() error {
	if y.Image == "" {
		return fmt.Errorf("[image] is required")
	}

	if y.Build != nil {
		if y.Build.Context == "" {
			return fmt.Errorf("[build.context] is required")
		}
	}

	if y.Volumes != nil && len(y.Volumes) > 0 {
		for k, v := range y.Volumes {
			if v.Path == nil && v.Name == nil {
				return fmt.Errorf("[volumes.%s] either [path] or [name] must be set", k)
			}

			if v.Path != nil && v.Name != nil {
				return fmt.Errorf("[volumes.%s] [path] and [name] cannot be set at the same time", k)
			}
		}
	}

	if y.Expose != nil && len(y.Expose) > 0 {
		for k, v := range y.Expose {
			if !regexExposeName.MatchString(k) {
				return fmt.Errorf("[expose.%s] must match regex %s", k, regexExposeName)
			}

			if v.Type == "" {
				return fmt.Errorf("[expose.%s.type] is required", k)
			} else if v.Type != "static" && v.Type != "grpc" && v.Type != "http" {
				return fmt.Errorf("[expose.%s.type] must be one of [static, grpc, http]", k)
			}

			if v.Type == "static" {
				if v.Port == nil {
					return fmt.Errorf("[expose.%s.port] is required", k)
				}
			}

			if v.Type == "http" {
				if v.Routes == nil || len(v.Routes) == 0 {
					return fmt.Errorf("[expose.%s.routes] is required", k)
				}
			} else {
				if v.Routes != nil && len(v.Routes) > 0 {
					return fmt.Errorf("[expose.%s.routes] is not allowed", k)
				}
			}
		}
	}

	if y.Attach != nil && len(y.Attach) > 0 {
		for i, a := range y.Attach {
			split := strings.Split(a, ":")
			if len(split) != 2 {
				return fmt.Errorf("[attach.%d] must be in the format [service:exposure]", i)
			}

			if split[0] == "" {
				return fmt.Errorf("[attach.%d] service name cannot be empty", i)
			}

			if split[1] == "" {
				return fmt.Errorf("[attach.%d] exposure name cannot be empty", i)
			}

			if regexExposeName.MatchString(split[1]) {
				return fmt.Errorf("[attach.%d] exposure name must match regex %s", i, regexExposeName)
			}
		}
	}

	return nil
}

func ParseServiceFilesInProject() (map[string]map[string]YamlEntry, error) {
	data := make(map[string]map[string]YamlEntry)

	wd, err := os.Getwd()
	if err != nil {
		panic(err)
	}

	err = recursivelyParseServiceFilesInProject(wd, data)
	if err != nil {
		return nil, err
	}

	mapUniqueServices := make(map[string]string)
	for fileName, services := range data {
		for serviceName := range services {
			if firstFileName, ok := mapUniqueServices[serviceName]; ok {
				return nil, fmt.Errorf("service %s is defined in both %s and %s", serviceName, firstFileName, fileName)
			}
			mapUniqueServices[serviceName] = fileName
		}
	}

	return data, nil
}

func recursivelyParseServiceFilesInProject(dir string, services map[string]map[string]YamlEntry) error {
	files, err := os.ReadDir(dir)
	if err != nil {
		log.Printf("failed to read directory %s: %v\n", dir, err)
	}

	for _, file := range files {
		if file.IsDir() {
			err := recursivelyParseServiceFilesInProject(dir+string(filepath.Separator)+file.Name(), services)
			if err != nil {
				return err
			}
		} else if file.Name() == "services.yaml" || file.Name() == "services.yml" {
			filePath := dir + string(filepath.Separator) + file.Name()
			content, err := os.ReadFile(filePath)
			if err != nil {
				return fmt.Errorf("failed to read file %s: %v", file.Name(), err)
			}
			data := make(map[string]YamlEntry)
			err = yaml.Unmarshal(content, &data)
			if err != nil {
				return fmt.Errorf("failed to unmarshal yaml file %s: %v", filePath, err)
			}
			services[filePath] = data
		}
	}

	return nil
}
