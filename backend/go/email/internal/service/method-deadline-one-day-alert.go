package service

import (
	"bytes"
	"context"
	"log/slog"
	"meowmentum/backend/email/internal/templates"
	pbEmail "meowmentum/backend/proto/email"
)

type DeadlineOneDayAlertProps struct {
	Name     string
	TaskName string
	TaskURL  string
}

func (s *emailServiceServer) DeadlineOneDayAlert(_ context.Context, req *pbEmail.DeadlineOneDayAlertRequest) (*pbEmail.DeadlineOneDayAlertResponse, error) {
	buffer := &bytes.Buffer{}

	err := templates.DeadlineOneDayAlertTemplate.Execute(buffer, DeadlineOneDayAlertProps{
		Name:     req.Name,
		TaskName: req.TaskName,
		TaskURL:  req.TaskURL,
	})
	if err != nil {
		slog.Error("failed to execute deadline one day alert template", slog.Any("error", err))
		return &pbEmail.DeadlineOneDayAlertResponse{}, nil
	}

	s.SendEmailBackground(req.Email, "Deadline alert", buffer.String())

	return &pbEmail.DeadlineOneDayAlertResponse{}, nil
}
