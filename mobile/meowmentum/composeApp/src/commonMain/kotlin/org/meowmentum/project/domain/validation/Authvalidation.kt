package org.meowmentum.project.domain.validation

import org.meowmentum.project.data.models.ValidationResult

object AuthValidation {
    fun validateEmail(email: String): ValidationResult {
        return when {
            email.isBlank() -> ValidationResult.Invalid("Email cannot be empty")
            !email.matches(Regex("^[A-Za-z0-9+_.-]+@(.+)\$")) ->
                ValidationResult.Invalid("Invalid email format")
            else -> ValidationResult.Valid
        }
    }

    fun validatePassword(password: String): ValidationResult {
        return when {
            password.length < 8 ->
                ValidationResult.Invalid("Password must be at least 8 characters")
            !password.any { it.isDigit() } ->
                ValidationResult.Invalid("Password must contain at least one number")
            !password.any { it.isUpperCase() } ->
                ValidationResult.Invalid("Password must contain at least one uppercase letter")
            else -> ValidationResult.Valid
        }
    }

    fun validateName(name: String): ValidationResult {
        return when {
            name.isBlank() -> ValidationResult.Invalid("Name cannot be empty")
            name.length < 2 -> ValidationResult.Invalid("Name is too short")
            else -> ValidationResult.Valid
        }
    }
}