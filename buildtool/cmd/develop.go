package main

import (
	"github.com/spf13/cobra"
	"log"
	"meowmentum/tools/buildtool/generators"
	"meowmentum/tools/buildtool/service"
	"os"
)

func init() {
	developCmd.Flags().StringVar(&developEnvironmentFlag, "env", "local", "Target environment [local, stage, production]")
	developCmd.Flags().BoolVar(&exposeAllPortsFlag, "expose-all", false, "Expose all ports")
	rootCmd.AddCommand(developCmd)
}

var developCmd = &cobra.Command{
	Use:   "develop [services to exclude]",
	Short: "Run local development environment",
	Run: func(cmd *cobra.Command, args []string) {
		developRun(cmd, args)
	},
}

var developEnvironmentFlag string
var exposeAllPortsFlag bool

func developRun(_ *cobra.Command, args []string) {
	_ = os.Setenv("ENVIRONMENT", developEnvironmentFlag)
	registry, err := service.LoadServiceRegistry(args, exposeAllPortsFlag, generators.GenerateAll)
	if err != nil {
		log.Fatalf("failed to load service registry: %v", err)
	}

	err = registry.WriteDockerCompose()
	if err != nil {
		log.Fatalf("failed to write docker-compose file: %v", err)
	}

	registry.RunApp()
}
