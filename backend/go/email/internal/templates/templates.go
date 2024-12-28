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

//go:embed deadline-one-day-alert.gohtml
var deadlineOneDayAlertTemplate string
var DeadlineOneDayAlertTemplate *template.Template

//go:embed overdue-alert.gohtml
var overdueAlertTemplate string
var OverdueAlertTemplate *template.Template

func init() {
	RegistrationConfirmationTemplate = template.Must(
		template.New("registration-confirmation").Parse(registrationConfirmationTemplate),
	)
	PasswordResetTemplate = template.Must(
		template.New("password-reset").Parse(passwordResetTemplate),
	)
	DeadlineOneDayAlertTemplate = template.Must(
		template.New("deadline-one-day-alert").Parse(deadlineOneDayAlertTemplate),
	)
	OverdueAlertTemplate = template.Must(
		template.New("overdue-alert").Parse(overdueAlertTemplate),
	)
}
