package utils

import (
	"os"
	"path/filepath"
	"strings"
)

func ResolveRelativePath(dirPath, path string) string {
	if strings.HasPrefix(path, "/") {
		wd, err := os.Getwd()
		if err != nil {
			panic(err)
		}

		path = filepath.Clean(wd + path)
	} else {
		path = filepath.Clean(dirPath + string(filepath.Separator) + path)
	}

	abs, err := filepath.Abs(path)
	if err != nil {
		panic(err)
	}

	return abs
}
