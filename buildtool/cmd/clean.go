package main

import (
	"github.com/spf13/cobra"
	"os"
	"path/filepath"
	"runtime"
	"slices"
)

func init() {
	rootCmd.AddCommand(cleanCmd)
}

var cleanCmd = &cobra.Command{
	Use:   "clean",
	Short: "Clean build directory",
	Run: func(cmd *cobra.Command, args []string) {
		cleanRun(cmd, args)
	},
}

func cleanRun(_ *cobra.Command, _ []string) {
	exeExt := ""

	if runtime.GOOS == "windows" {
		exeExt = ".exe"
	}

	preserveFiles := []string{
		"./build/buildtool" + exeExt,
		"./build/hash",
	}

	for i := range preserveFiles {
		var err error
		preserveFiles[i], err = filepath.Abs(preserveFiles[i])
		if err != nil {
			panic(err)
		}
	}

	buildDir, err := filepath.Abs("./build")
	if err != nil {
		panic(err)
	}

	_, err = removeDirectoryRecursiveWithPreserve(buildDir, preserveFiles)
	if err != nil {
		panic(err)
	}
}

func removeDirectoryRecursiveWithPreserve(dir string, preserveFiles []string) (hadPreservedFiles bool, err error) {
	files, err := os.ReadDir(dir)
	if err != nil {
		return false, err
	}

	for _, file := range files {
		filePath := filepath.Join(dir, file.Name())
		if slices.Contains(preserveFiles, filePath) {
			hadPreservedFiles = true
			continue
		}

		if file.IsDir() {
			hadPreservedFiles, err = removeDirectoryRecursiveWithPreserve(filePath, preserveFiles)
			if err != nil {
				return false, err
			}

			if !hadPreservedFiles {
				err := os.Remove(filePath)
				if err != nil {
					return false, err
				}
			}
		} else {
			err := os.Remove(filePath)
			if err != nil {
				return false, err
			}
		}
	}

	return
}
