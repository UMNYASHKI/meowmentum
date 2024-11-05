package templates

import (
	_ "embed"
	"html/template"
)

//go:embed registration-confirmation.gohtml
var registrationConfirmationTemplate string
var RegistrationConfirmationTemplate *template.Template

//go:embed password-reset.gohtml
var passwordResetTemplate string
var PasswordResetTemplate *template.Template

func init() {
	RegistrationConfirmationTemplate = template.Must(
		template.New("registration-confirmation").Parse(registrationConfirmationTemplate),
	)
	PasswordResetTemplate = template.Must(
		template.New("password-reset").Parse(passwordResetTemplate),
	)
}
