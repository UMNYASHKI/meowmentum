package org.meowmentum.project.domain.validation

sealed class ValidationResult {
    object Valid : ValidationResult()
    data class Invalid(val message: String) : ValidationResult()
}

object AuthValidation {
    fun validateEmail(email: String): ValidationResult {
        return when {
            email.isBlank() -> ValidationResult.Invalid("Email cannot be empty")
            !email.matches(Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}\$")) ->
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
            !password.any { it.isLowerCase() } ->
                ValidationResult.Invalid("Password must contain at least one lowercase letter")
            !password.any { !it.isLetterOrDigit() } ->
                ValidationResult.Invalid("Password must contain at least one special character")
            else -> ValidationResult.Valid
        }
    }

    fun validateName(name: String): ValidationResult {
        return when {
            name.isBlank() -> ValidationResult.Invalid("Name cannot be empty")
            name.length < 2 -> ValidationResult.Invalid("Name is too short")
            name.any { !it.isLetterOrDigit() && it != ' ' } ->
                ValidationResult.Invalid("Name can only contain letters, numbers, and spaces")
            else -> ValidationResult.Valid
        }
    }
}