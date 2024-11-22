package config

import (
	"go-simpler.org/env"
	"time"
)

type Environment string

const (
	EnvironmentLocal      Environment = "local"
	EnvironmentProduction Environment = "production"
)

type Config interface {
	GetCommonConfig() CommonConfig
}

type CommonConfig struct {
	Environment             Environment   `env:"ENVIRONMENT" default:"local"`
	GracefulShutdownTimeout time.Duration `env:"CONFIG_GRACEFUL_SHUTDOWN_TIMEOUT" default:"5s"`
	Logging                 struct {
		AttachLoki string `env:"ATTACH_STATIC_LOKI_HTTP"`
	}
}

func (c CommonConfig) GetCommonConfig() CommonConfig {
	return c
}

func Load(cfg Config) error {
	err := env.Load(cfg, nil)
	if err != nil {
		return err
	}

	return nil
}
