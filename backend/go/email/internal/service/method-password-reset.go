package service

import (
	"bytes"
	"context"
	"log/slog"
	"meowmentum/backend/email/internal/templates"
	pbEmail "meowmentum/backend/proto/email"
)

type PasswordResetProps struct {
	Name string
	Code string
}

func (s *emailServiceServer) PasswordReset(_ context.Context, req *pbEmail.PasswordResetRequest) (*pbEmail.PasswordResetResponse, error) {
	buffer := &bytes.Buffer{}

	err := templates.PasswordResetTemplate.Execute(buffer, PasswordResetProps{
		Name: req.Name,
		Code: req.ConfirmationCode,
	})
	if err != nil {
		slog.Error("failed to execute password reset template", slog.Any("error", err))
		return &pbEmail.PasswordResetResponse{}, nil
	}

	s.SendEmailBackground(req.Email, "Registration Confirmation", buffer.String())

	return &pbEmail.PasswordResetResponse{}, nil
}
