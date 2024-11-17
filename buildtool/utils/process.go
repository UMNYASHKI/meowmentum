package utils

import (
	"os"
	"os/exec"
)

func RunBash(command string) (string, error) {
	cmd := exec.Command("bash", "-c", command)
	cmd.Stderr = os.Stderr
	out, err := cmd.Output()
	if err != nil {
		return "", err
	}
	return string(out), nil
}
