package smtp

import (
	"gopkg.in/gomail.v2"
	"meowmentum/backend/email/internal/config"
)

type Sender struct {
	host     string
	port     int
	username string
	password string
	from     string
}

func NewSender(config *config.ServiceConfig) *Sender {
	return &Sender{
		host:     config.Secret.EmailSMTP.Host,
		port:     config.Secret.EmailSMTP.Port,
		username: config.Secret.EmailSMTP.Username,
		password: config.Secret.EmailSMTP.Password,
		from:     config.Secret.EmailSMTP.From,
	}
}

func (s *Sender) SendEmail(to, subject, body string) error {
	message := gomail.NewMessage()

	message.SetHeader("From", s.from)
	message.SetHeader("To", to)
	message.SetHeader("Subject", subject)

	message.SetBody("text/html; charset=UTF-8", body)

	dialer := gomail.NewDialer(s.host, s.port, s.username, s.password)

	return dialer.DialAndSend(message)
}
