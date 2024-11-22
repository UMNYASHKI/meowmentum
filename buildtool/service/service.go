package service

import (
	"fmt"
	"gopkg.in/yaml.v3"
	"meowmentum/tools/buildtool/utils"
	"os"
	"path/filepath"
	"sort"
	"strings"
)

type Service struct {
	Name      string
	DefinedIn string
	Directory string
	Yaml      YamlEntry
	Excluded  bool

	Image   string
	Build   *BuildConfig
	Volumes map[string]Volume
	Labels  map[string]string
	Expose  map[string]*Expose
	Attach  []*Attach
	EnvMap  map[string]string
	EnvPass []string
}

func (s *Service) GetComposeServiceName() string {
	return fmt.Sprintf("%s.%s", os.Getenv("ENVIRONMENT"), s.Name)
}

func FromYamlEntry(filePath, name string, yaml YamlEntry, isExcluded bool) (*Service, error) {
	_ = os.Setenv("SERVICE", strings.ReplaceAll(name, ".", "-"))
	defer os.Unsetenv("SERVICE")

	service := Service{
		Name:      name,
		DefinedIn: filePath,
		Directory: filepath.Dir(filePath),
		Yaml:      yaml,
		Excluded:  isExcluded,
		Image:     os.ExpandEnv(yaml.Image),
		Volumes:   make(map[string]Volume),
		Labels:    make(map[string]string),
		Expose:    make(map[string]*Expose),
		Attach:    make([]*Attach, 0),
		EnvMap:    make(map[string]string),
		EnvPass:   make([]string, 0),
	}

	if yaml.EnvPass != nil {
		for _, env := range yaml.EnvPass {
			service.EnvPass = append(service.EnvPass, os.ExpandEnv(env))
		}
	}

	if yaml.EnvMap != nil {
		for k, v := range yaml.EnvMap {
			service.EnvMap[k] = v
		}
	}

	if yaml.Build != nil {
		service.Build = &BuildConfig{
			Args:               make(map[string]string),
			AdditionalContexts: make(map[string]string),
		}

		{
			ctxExp := os.ExpandEnv(yaml.Build.Context)
			if !strings.HasPrefix(ctxExp, "gen:") {
				ctxExp = utils.ResolveRelativePath(service.Directory, ctxExp)
			}
			service.Build.Context = ctxExp
		}

		if yaml.Build.Dockerfile != nil {
			path := os.ExpandEnv(utils.ResolveRelativePath(service.Directory, *yaml.Build.Dockerfile))
			service.Build.Dockerfile = &path
		}

		if yaml.Build.Args != nil {
			for k, v := range yaml.Build.Args {
				service.Build.Args[k] = os.ExpandEnv(v)
			}
		}

		if yaml.Build.AdditionalContexts != nil {
			for k, v := range yaml.Build.AdditionalContexts {
				ctxExp := os.ExpandEnv(v)
				if !strings.HasPrefix(ctxExp, "gen:") {
					ctxExp = utils.ResolveRelativePath(service.Directory, ctxExp)
				}
				service.Build.AdditionalContexts[k] = ctxExp
			}
		}

		if yaml.Build.Target != nil {
			expanded := os.ExpandEnv(*yaml.Build.Target)
			service.Build.Target = &expanded
		}
	}

	if yaml.Volumes != nil && len(yaml.Volumes) > 0 {
		for k, v := range yaml.Volumes {
			if v.Path != nil {
				service.Volumes[k] = &PathVolume{Path: os.ExpandEnv(utils.ResolveRelativePath(service.Directory, *v.Path))}
			} else if v.Name != nil {
				vol := &NameVolume{Name: os.ExpandEnv(*v.Name)}

				if v.Config.Kind != 0 {
					vol.VolumeConfig = &v.Config
				}

				service.Volumes[k] = vol
			}
		}
	}

	if yaml.Labels != nil {
		for k, v := range yaml.Labels {
			service.Labels[k] = os.ExpandEnv(v)
		}
	}

	if yaml.Expose != nil && len(yaml.Expose) > 0 {
		keys := make([]string, 0, len(yaml.Expose))
		for k := range yaml.Expose {
			keys = append(keys, k)
		}
		sort.Strings(keys)
		port := 50000
		for _, k := range keys {
			v := yaml.Expose[k]
			expose := &Expose{
				ComposeServiceExcluded: service.Excluded,
				ComposeServiceAddress:  service.GetComposeServiceName(),
				Name:                   k,
			}

			if v.Port != nil {
				expose.InnerPort = *v.Port
			} else {
				expose.InnerPort = port
				port++
			}

			if v.Pin != nil {
				expose.OuterPort = *v.Pin
				expose.IsExposed = true
			}

			switch v.Type {
			case "static":
				expose.Type = StaticExpose{}
			case "grpc":
				expose.Type = GrpcExpose{}
			case "http":
				expose.Type = HttpExpose{Routes: v.Routes}
			}

			service.Expose[k] = expose
		}
	}

	if yaml.Attach != nil && len(yaml.Attach) > 0 {
		for _, attach := range yaml.Attach {
			parts := strings.Split(attach, ":")
			if len(parts) != 2 {
				return nil, fmt.Errorf("invalid attach format: %s", attach)
			}

			service.Attach = append(service.Attach, &Attach{
				AttachService: parts[0],
				AttachTo:      parts[1],
			})
		}
	}

	return &service, nil
}

// Build

type BuildConfig struct {
	Context            string
	Dockerfile         *string
	Args               map[string]string
	AdditionalContexts map[string]string
	Target             *string
}

// Volumes

type Volume interface {
	String() string
}

type PathVolume struct {
	Path string
}

func (v PathVolume) String() string {
	return v.Path
}

type NameVolume struct {
	Name         string
	VolumeConfig *yaml.Node
}

func (v NameVolume) String() string {
	return v.Name
}

// Expose

type Expose struct {
	ComposeServiceExcluded bool
	ComposeServiceAddress  string
	Name                   string
	Type                   ExposeType
	InnerPort              int
	OuterPort              int
	IsExposed              bool
}

func (e *Expose) GetComposeServiceAddress() string {
	if e.ComposeServiceExcluded {
		return fmt.Sprintf("host.docker.internal:%d", e.OuterPort)
	} else {
		return fmt.Sprintf("%s:%d", e.ComposeServiceAddress, e.InnerPort)
	}
}

type ExposeType interface {
	Name() string
	CanBeAttached() bool
	GetAttachmentVar(exposureName string) string
	GetExposureVar(exposureName string) string
}

type StaticExpose struct{}

func (e StaticExpose) Name() string {
	return "static"
}

func (e StaticExpose) CanBeAttached() bool {
	return true
}

func (e StaticExpose) GetAttachmentVar(exposureName string) string {
	if exposureName == "static" {
		return "ATTACH_STATIC"
	}

	return fmt.Sprintf("ATTACH_STATIC_%s", strings.ToUpper(strings.ReplaceAll(exposureName, "-", "_")))
}

func (e StaticExpose) GetExposureVar(exposureName string) string {
	if exposureName == "static" {
		return "EXPOSE_STATIC"
	}

	return fmt.Sprintf("EXPOSE_STATIC_%s", strings.ToUpper(strings.ReplaceAll(exposureName, "-", "_")))
}

type GrpcExpose struct{}

func (e GrpcExpose) Name() string {
	return "grpc"
}

func (e GrpcExpose) CanBeAttached() bool {
	return true
}

func (e GrpcExpose) GetAttachmentVar(exposureName string) string {
	if exposureName == "grpc" {
		return "ATTACH_GRPC"
	}

	return fmt.Sprintf("ATTACH_GRPC_%s", strings.ToUpper(strings.ReplaceAll(exposureName, "-", "_")))
}

func (e GrpcExpose) GetExposureVar(exposureName string) string {
	if exposureName == "grpc" {
		return "EXPOSE_GRPC"
	}

	return fmt.Sprintf("EXPOSE_GRPC_%s", strings.ToUpper(strings.ReplaceAll(exposureName, "-", "_")))
}

type HttpExpose struct {
	Routes map[string]string
}

func (e HttpExpose) Name() string {
	return "http"
}

func (e HttpExpose) CanBeAttached() bool {
	return false
}

func (e HttpExpose) GetAttachmentVar(_ string) string {
	return ""
}

func (e HttpExpose) GetExposureVar(exposureName string) string {
	if exposureName == "http" {
		return "EXPOSE_HTTP"
	}

	return fmt.Sprintf("EXPOSE_HTTP_%s", strings.ToUpper(strings.ReplaceAll(exposureName, "-", "_")))
}

// Attach

type Attach struct {
	AttachService string
	AttachTo      string

	ResolvedAttachService *Service
	ResolvedAttachTo      *Expose
}
