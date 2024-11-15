package generators

import (
	"fmt"
	"github.com/google/shlex"
	"meowmentum/tools/buildtool/service"
	"os"
	"path/filepath"
	"strings"
)

var generators = make(map[string]Generator)

type Generator interface {
	Generate(outDir string, registry *service.Registry, service *service.Service, args []string) error
}

func GenerateAll(registry *service.Registry) error {
	for _, k := range registry.SortedKeys {
		s := registry.Services[k]
		if s.Build != nil {
			out, err := generateIfNeeded(registry, s, s.Build.Context)
			if err != nil {
				return err
			}

			if out != "" {
				s.Build.Context = out
			}

			for k, v := range s.Build.AdditionalContexts {
				out, err := generateIfNeeded(registry, s, v)
				if err != nil {
					return err
				}

				if out != "" {
					s.Build.AdditionalContexts[k] = out
				}
			}
		}
	}

	return nil
}

func generateIfNeeded(registry *service.Registry, service *service.Service, pathString string) (string, error) {
	if !strings.HasPrefix(pathString, "gen:") {
		return "", nil
	}

	parts, err := shlex.Split(pathString)
	if err != nil {
		return "", err
	}

	args := parts[1:]
	genName := parts[0][4:]

	gen, ok := generators[genName]
	if !ok {
		return "", fmt.Errorf("unknown generator: %s", genName)
	}

	outDir, err := filepath.Abs(fmt.Sprintf("./build/gen/%s/%s", service.Name, genName))
	if err != nil {
		return "", err
	}

	_ = os.MkdirAll(outDir, os.ModePerm)

	err = gen.Generate(outDir, registry, service, args)
	if err != nil {
		return "", fmt.Errorf("failed to generate %s for %s: %w", genName, service.Name, err)
	}

	return outDir, nil
}
