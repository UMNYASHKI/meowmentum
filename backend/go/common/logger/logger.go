package logger

import (
	"github.com/lmittmann/tint"
	"github.com/mattn/go-isatty"
	"log/slog"
	"meowmentum/backend/common/config"
	"os"
)

func SetupLogger(_ config.Config) {
	// TODO: Implement environment-dependant logger setup
	log := slog.New(tint.NewHandler(os.Stdout, &tint.Options{Level: slog.LevelDebug, NoColor: !isatty.IsTerminal(os.Stderr.Fd())}))
	slog.SetDefault(log)
}
