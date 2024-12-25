package service

import (
	"bytes"
	"context"
	"fmt"
	"log/slog"
	"meowmentum/backend/email/internal/templates"
	pbEmail "meowmentum/backend/proto/email"
)

type OverdueAlertProps struct {
	Name      string
	TaskCount int32
}

func (s *emailServiceServer) OverdueAlert(_ context.Context, req *pbEmail.OverdueAlertRequest) (*pbEmail.OverdueAlertResponse, error) {
	buffer := &bytes.Buffer{}

	err := templates.OverdueAlertTemplate.Execute(buffer, OverdueAlertProps{
		Name:      req.Name,
		TaskCount: req.TaskCount,
	})
	if err != nil {
		slog.Error("failed to execute overdue alert template", slog.Any("error", err))
		return &pbEmail.OverdueAlertResponse{}, nil
	}

	s.SendEmailBackground(req.Email, fmt.Sprintf("You have %d task(s) overdue!", req.TaskCount), buffer.String())

	return &pbEmail.OverdueAlertResponse{}, nil
}
