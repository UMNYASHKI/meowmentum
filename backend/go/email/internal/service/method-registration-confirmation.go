package service

import (
	"bytes"
	"context"
	"log/slog"
	"meowmentum/backend/email/internal/templates"
	pbEmail "meowmentum/backend/proto/email"
)

type RegistrationConfirmationProps struct {
	Name string
	Code string
}

func (s *emailServiceServer) RegistrationConfirmation(_ context.Context, req *pbEmail.RegistrationConfirmationRequest) (*pbEmail.RegistrationConfirmationResponse, error) {
	buffer := &bytes.Buffer{}

	err := templates.RegistrationConfirmationTemplate.Execute(buffer, RegistrationConfirmationProps{
		Name: req.Name,
		Code: req.ConfirmationCode,
	})
	if err != nil {
		slog.Error("failed to execute registration confirmation template", slog.Any("error", err))
		return &pbEmail.RegistrationConfirmationResponse{}, nil
	}

	err = s.sender.SendEmail(req.Email, "Registration Confirmation", buffer.String())
	if err != nil {
		slog.Error("failed to send registration confirmation email", slog.Any("error", err))
	}

	return &pbEmail.RegistrationConfirmationResponse{}, nil
}
