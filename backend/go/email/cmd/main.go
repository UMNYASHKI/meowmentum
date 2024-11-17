package main

import (
	"fmt"
	"log/slog"
	"meowmentum/backend/common/lifecycle"
	"meowmentum/backend/common/logger"
	"meowmentum/backend/email/internal/config"
	"meowmentum/backend/email/internal/senders/smtp"
	"meowmentum/backend/email/internal/service"
	"os"
)

func main() {
	cfg, err := config.LoadServiceConfig()
	if err != nil {
		panic(err)
	}

	lc := lifecycle.NewLifecycle(cfg)

	logger.SetupLogger(cfg, "service.go.email")

	if err := setupService(lc, cfg); err != nil {
		slog.Error("failed to setup service", slog.Any("config", cfg), slog.Any("error", err))
		os.Exit(1)
	}

	slog.Info("service started")

	lc.Wait()
}

func setupService(lc *lifecycle.Lifecycle, cfg *config.ServiceConfig) error {
	sender := smtp.NewSender(cfg)

	_, err := service.NewEmailServiceServer(lc, cfg, sender)
	if err != nil {
		return fmt.Errorf("failed to create email service server: %w", err)
	}

	return nil
}
