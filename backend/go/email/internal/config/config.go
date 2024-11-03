package config

import (
	"meowmentum/backend/common/config"
)

type ServiceConfig struct {
	config.CommonConfig

	Expose struct {
		Grpc struct {
			Email string `env:"EXPOSE_GRPC_EMAIL,required"`
		}
	}

	Secret struct {
		EmailSMTP struct {
			Host     string `env:"SECRET_EMAIL_SMTP_HOST,required"`
			Port     int    `env:"SECRET_EMAIL_SMTP_PORT,required"`
			Username string `env:"SECRET_EMAIL_SMTP_USERNAME,required"`
			Password string `env:"SECRET_EMAIL_SMTP_PASSWORD,required"`
			From     string `env:"SECRET_EMAIL_SMTP_FROM,required"`
		}
	}
}

func LoadServiceConfig() (*ServiceConfig, error) {
	cfg := &ServiceConfig{}

	err := config.Load(cfg)
	if err != nil {
		return nil, err
	}

	return cfg, nil
}
