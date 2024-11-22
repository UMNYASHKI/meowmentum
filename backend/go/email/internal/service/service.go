package service

import (
	"fmt"
	"github.com/grpc-ecosystem/go-grpc-middleware/v2/interceptors/logging"
	"google.golang.org/grpc"
	"log/slog"
	"meowmentum/backend/common/lifecycle"
	"meowmentum/backend/common/logger"
	"meowmentum/backend/email/internal/config"
	"meowmentum/backend/email/internal/senders"
	pbEmail "meowmentum/backend/proto/email"
	"net"
)

type emailServiceServer struct {
	pbEmail.UnimplementedEmailServiceServer
	sender senders.EmailSender
}

func NewEmailServiceServer(
	lc *lifecycle.Lifecycle,
	config *config.ServiceConfig,
	sender senders.EmailSender,
) (pbEmail.EmailServiceServer, error) {
	server := &emailServiceServer{sender: sender}

	slog.Debug("starting grpc listener", slog.String("address", config.Expose.Grpc.Email))
	lis, err := net.Listen("tcp", config.Expose.Grpc.Email)
	if err != nil {
		return nil, fmt.Errorf("failed to listen: %w", err)
	}

	lc.AddModule("grpc-server-email")

	grpcLogger := logger.GetGrpcLogger("email")
	opts := []logging.Option{
		logging.WithLogOnEvents(logging.StartCall, logging.FinishCall, logging.PayloadSent, logging.PayloadReceived),
	}
	grpcServer := grpc.NewServer(
		grpc.ChainUnaryInterceptor(
			logging.UnaryServerInterceptor(grpcLogger, opts...),
		),
		grpc.ChainStreamInterceptor(
			logging.StreamServerInterceptor(grpcLogger, opts...),
		),
	)
	pbEmail.RegisterEmailServiceServer(grpcServer, server)

	go func() {
		err := grpcServer.Serve(lis)
		if err != nil {
			panic(err)
		}

		lc.DoneModule("grpc-server-email")
	}()

	go func() {
		<-lc.Context().Done()
		grpcServer.GracefulStop()
	}()

	return server, nil
}

func (s *emailServiceServer) SendEmailBackground(to, subject, body string) {
	go func() {
		err := s.sender.SendEmail(to, subject, body)
		if err != nil {
			slog.Error("failed to send email",
				slog.Any("error", err),
				slog.String("to", to),
				slog.String("subject", subject),
			)
		}
	}()
}
