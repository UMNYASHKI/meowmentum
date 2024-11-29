package org.meowmentum.project.data.models

import kotlinx.serialization.Serializable

@Serializable
data class LoginRequest(
    val email: String,
    val password: String
)

@Serializable
data class LoginResponse(
    val token: String
)

@Serializable
data class RegisterUserRequest(
    val email: String,
    val password: String,
    val name: String
)

@Serializable
data class OtpValidationRequest(
    val email: String,
    val code: String
)

@Serializable
data class PasswordResetRequest(
    val email: String
)

@Serializable
data class PasswordUpdateRequest(
    val email: String,
    val token: String,
    val newPassword: String
)

@Serializable
data class UserDto(
    val id: String,
    val email: String,
    val name: String
)

@Serializable
data class ResetPasswordResponse(
    val token: String
)