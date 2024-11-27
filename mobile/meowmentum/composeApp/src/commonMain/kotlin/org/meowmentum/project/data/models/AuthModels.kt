package org.meowmentum.project.data.models

import kotlinx.serialization.Serializable

@Serializable
data class LoginCredentials(
    val email: String,
    val password: String
)

@Serializable
data class RegisterCredentials(
    val name: String,
    val email: String,
    val password: String
)

@Serializable
data class AuthResponse(
    val token: String,
    val refreshToken: String,
    val user: UserDto
)

@Serializable
data class UserDto(
    val id: String,
    val email: String,
    val name: String
)

@Serializable
sealed class AuthResult {
    data class Success(val user: UserDto) : AuthResult()
    data class Error(val message: String) : AuthResult()
}

