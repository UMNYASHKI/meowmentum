package logger

import (
	"context"
	"github.com/grpc-ecosystem/go-grpc-middleware/v2/interceptors/logging"
	"github.com/lmittmann/tint"
	slogloki "github.com/magnetde/slog-loki"
	"github.com/mattn/go-isatty"
	"github.com/samber/slog-multi"
	"io"
	"log/slog"
	"meowmentum/backend/common/config"
	"os"
)

func SetupLogger(cfg config.Config, logServiceName string) {
	commonCfg := cfg.GetCommonConfig()

	handlers := make([]slog.Handler, 0)
	logLevel := slog.LevelInfo

	if commonCfg.Environment == config.EnvironmentLocal {
		logLevel = slog.LevelDebug
	}

	if commonCfg.Environment == config.EnvironmentLocal && isatty.IsTerminal(os.Stderr.Fd()) {
		handlers = append(handlers, tint.NewHandler(os.Stderr, &tint.Options{Level: slog.LevelDebug}))
	} else {
		handlers = append(handlers, slog.NewTextHandler(os.Stderr, &slog.HandlerOptions{Level: logLevel}))
	}

	if commonCfg.Logging.AttachLoki != "" {
		handler := slogloki.NewHandler(
			"http://"+commonCfg.Logging.AttachLoki,
			slogloki.WithHandler(func(w io.Writer) slog.Handler {
				return slog.NewTextHandler(w, &slog.HandlerOptions{
					Level: logLevel,
				})
			}),
			slogloki.WithLabel("source", "direct"),
			slogloki.WithLabel("service", logServiceName),
		)
		handlers = append(handlers, handler)
	}

	slog.SetDefault(slog.New(slogmulti.Fanout(handlers...)))
}

func GetGrpcLogger(serverName string) logging.Logger {
	return logging.LoggerFunc(func(ctx context.Context, lvl logging.Level, msg string, fields ...any) {
		l := slog.Default().With(
			slog.String("grpc_server", serverName),
		)
		l.Log(ctx, slog.Level(lvl), msg, fields...)
	})
}
