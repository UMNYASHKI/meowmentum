package org.meowmentum.project.data.remote

import org.meowmentum.project.data.models.*

interface AuthApi {
    suspend fun register(request: RegisterUserRequest): String
    suspend fun verifyOtp(request: OtpValidationRequest): String
    suspend fun login(request: LoginRequest): LoginResponse
    suspend fun sendResetOtp(email: String): String
    suspend fun verifyResetOtp(request: OtpValidationRequest): ResetPasswordResponse
    suspend fun resetPassword(request: PasswordUpdateRequest): String
    suspend fun logout(): String
}