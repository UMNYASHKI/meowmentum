package generators

import (
	"fmt"
	"meowmentum/tools/buildtool/service"
	"meowmentum/tools/buildtool/utils"
	"os"
	"path/filepath"
	"text/template"
)

func init() {
	generators["nginx"] = &Nginx{}
}

type Nginx struct{}

func (gen *Nginx) Generate(outDir string, registry *service.Registry, s *service.Service, args []string) error {
	if len(args) != 1 {
		return fmt.Errorf("expected 1 argument, got %d", len(args))
	}

	templatePath := utils.ResolveRelativePath(s.Directory, args[0])

	content, err := os.ReadFile(templatePath)
	if err != nil {
		return fmt.Errorf("failed to read nginx template: %w", err)
	}

	type nginxRoute struct {
		Path      string
		ProxyPass string
	}

	type nginxCtx struct {
		Port   int
		Routes []nginxRoute
	}

	nginxConfigTemplate := string(content)

	data := nginxCtx{
		Port:   8080,
		Routes: []nginxRoute{},
	}

	for _, k := range registry.SortedKeys {
		s := registry.Services[k]
		for _, e := range s.Expose {
			if v, ok := e.Type.(service.HttpExpose); ok {
				for from, to := range v.Routes {
					data.Routes = append(data.Routes, nginxRoute{
						Path:      from,
						ProxyPass: "http://" + e.GetComposeServiceAddress() + to,
					})
				}
			}
		}
	}

	tpl, err := template.New("nginx").Parse(nginxConfigTemplate)
	if err != nil {
		return fmt.Errorf("failed to parse nginx template: %w", err)
	}

	file, err := os.OpenFile(filepath.Join(outDir, "nginx.conf"), os.O_CREATE|os.O_WRONLY|os.O_TRUNC, os.ModePerm)
	if err != nil {
		return fmt.Errorf("failed to create nginx config file: %w", err)
	}

	defer file.Close()

	err = tpl.Execute(file, data)
	if err != nil {
		return fmt.Errorf("failed to execute nginx template: %w", err)
	}

	return nil
}
