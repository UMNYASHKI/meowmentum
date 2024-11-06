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

data class AuthResponse(
    val token: String,
    val refreshToken: String,
    val user: UserDto
)

data class UserDto(
    val id: String,
    val email: String,
    val name: String
)

sealed class AuthResult {
    data class Success(val user: UserDto) : AuthResult()
    data class Error(val message: String) : AuthResult()
}

