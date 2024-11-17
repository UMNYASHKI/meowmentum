package service

import (
	"fmt"
	"google.golang.org/grpc"
	"meowmentum/backend/common/lifecycle"
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

	lis, err := net.Listen("tcp", config.Expose.Grpc.Email)
	if err != nil {
		return nil, fmt.Errorf("failed to listen: %w", err)
	}

	lc.AddModule("grpc-server-email")

	grpcServer := grpc.NewServer()
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
