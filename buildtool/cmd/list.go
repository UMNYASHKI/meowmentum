package main

import (
	"github.com/spf13/cobra"
	"log"
	"meowmentum/tools/buildtool/service"
	"sort"
)

func init() {
	rootCmd.AddCommand(listCmd)
}

var listCmd = &cobra.Command{
	Use:   "list",
	Short: "List services",
	Run: func(cmd *cobra.Command, args []string) {
		listRun(cmd, args)
	},
}

func listRun(_ *cobra.Command, _ []string) {
	data, err := service.ParseServiceFilesInProject()
	if err != nil {
		log.Fatalf("failed to parse service files in project: %v", err)
	}

	maxServiceNameLength := 0
	mapServiceNameToFiles := make(map[string]string)
	serviceKeys := make([]string, 0)

	for fileName, services := range data {
		for serviceName := range services {
			if len(serviceName) > maxServiceNameLength {
				maxServiceNameLength = len(serviceName)
			}
			mapServiceNameToFiles[serviceName] += fileName
			serviceKeys = append(serviceKeys, serviceName)
		}
	}

	sort.Strings(serviceKeys)

	if len(serviceKeys) == 0 {
		log.Println("No services found")
		return
	}

	log.Printf("%-*s %s\n", maxServiceNameLength, "Service", "File")
	for _, serviceName := range serviceKeys {
		fileNames := mapServiceNameToFiles[serviceName]
		log.Printf("%-*s %s\n", maxServiceNameLength, serviceName, fileNames)
	}
}
