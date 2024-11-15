package main

import (
	"github.com/spf13/cobra"
	"log"
	"os"
)

var rootCmd = &cobra.Command{
	Use:   "meowmentum",
	Short: "Buildtool for meowmentum",
	CompletionOptions: cobra.CompletionOptions{
		DisableDefaultCmd: true,
	},
}

func main() {
	if os.Getenv("GIT_COMMIT") == "" {
		log.Fatalf("GIT_COMMIT environment variable was not set")
	}

	err := rootCmd.Execute()
	if err != nil {
		log.Fatalf("failed to execute root command: %v", err)
	}
}
