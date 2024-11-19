package main

import (
	"github.com/spf13/cobra"
	"log"
	"meowmentum/tools/buildtool/generators"
	"meowmentum/tools/buildtool/service"
	"os"
)

func init() {
	buildCmd.Flags().StringVar(&buildEnvironmentFlag, "env", "", "Target environment [local, stage, production] (required)")
	_ = buildCmd.MarkFlagRequired("env")
	buildCmd.Flags().BoolVar(&buildExportFlag, "export", false, "Export built images to build/images/{env}-{commit}-images.tar")
	rootCmd.AddCommand(buildCmd)
}

var buildCmd = &cobra.Command{
	Use:   "build",
	Short: "Build deployment artifacts",
	Run: func(cmd *cobra.Command, args []string) {
		buildRun(cmd, args)
	},
}

var buildEnvironmentFlag string
var buildExportFlag bool

func buildRun(_ *cobra.Command, _ []string) {
	_ = os.Setenv("ENVIRONMENT", buildEnvironmentFlag)
	registry, err := service.LoadServiceRegistry([]string{}, exposeAllPortsFlag, generators.GenerateAll)
	if err != nil {
		log.Fatalf("failed to load service registry: %v", err)
	}

	err = registry.WriteDockerCompose()
	if err != nil {
		log.Fatalf("failed to write docker-compose file: %v", err)
	}

	registry.BuildApp()
}
