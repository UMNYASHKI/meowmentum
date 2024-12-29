package org.meowmentum.project.data.remote.auth

import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult

interface AuthApi {
    suspend fun register(request: RegisterUserRequest): ApiResult<String>
    suspend fun verifyOtp(request: OtpValidationRequest): ApiResult<String>
    suspend fun login(request: LoginRequest): ApiResult<LoginResponse>
    suspend fun sendResetOtp(request: PasswordResetRequest): ApiResult<String>
    suspend fun verifyResetOtp(request: OtpValidationRequest): ApiResult<ResetPasswordResponse>
    suspend fun resetPassword(request: PasswordUpdateRequest): ApiResult<String>
    suspend fun logout(): ApiResult<String>
}