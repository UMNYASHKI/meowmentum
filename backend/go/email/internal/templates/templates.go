package templates

import (
	_ "embed"
	"html/template"
)

//go:embed registration-confirmation.gohtml
var registrationConfirmationTemplate string
var RegistrationConfirmationTemplate *template.Template

func init() {
	RegistrationConfirmationTemplate = template.Must(
		template.New("registration-confirmation").Parse(registrationConfirmationTemplate),
	)
}
