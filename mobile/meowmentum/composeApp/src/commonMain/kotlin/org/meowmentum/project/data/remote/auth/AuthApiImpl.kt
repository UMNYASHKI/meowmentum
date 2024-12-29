package org.meowmentum.project.data.remote.auth

import io.ktor.client.*
import io.ktor.client.request.post
import io.ktor.client.request.setBody
import io.ktor.http.ContentType
import io.ktor.http.contentType
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.data.remote.core.handleApiResponse

class AuthApiImpl(
    private val client: HttpClient,
    private val baseUrl: String = "http://10.0.2.2:8080/api/core/api/auth"
) : AuthApi {
    override suspend fun register(request: RegisterUserRequest): ApiResult<String> =
        handleApiResponse {
            client.post("$baseUrl/register") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun verifyOtp(request: OtpValidationRequest): ApiResult<String> =
        handleApiResponse {
            client.post("$baseUrl/verify-otp") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun login(request: LoginRequest): ApiResult<LoginResponse> =
        handleApiResponse {
            client.post("$baseUrl/login") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun sendResetOtp(request: PasswordResetRequest): ApiResult<String> =
        handleApiResponse {
            client.post("$baseUrl/send-reset-otp") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun verifyResetOtp(request: OtpValidationRequest): ApiResult<ResetPasswordResponse> =
        handleApiResponse {
            client.post("$baseUrl/verify-reset-otp") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun resetPassword(request: PasswordUpdateRequest): ApiResult<String> =
        handleApiResponse {
            client.post("$baseUrl/reset-password") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun logout(): ApiResult<String> =
        handleApiResponse {
            client.post("$baseUrl/logout")
        }
}