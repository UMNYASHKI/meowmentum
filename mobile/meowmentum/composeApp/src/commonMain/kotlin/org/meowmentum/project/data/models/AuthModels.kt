package org.meowmentum.project.data.models

data class LoginCredentials(
    val email: String,
    val password: String
)

data class RegisterCredentials(
    val name: String,
    val email: String,
    val password: String
)

sealed class ValidationResult {
    object Valid : ValidationResult()
    data class Invalid(val message: String) : ValidationResult()
}
