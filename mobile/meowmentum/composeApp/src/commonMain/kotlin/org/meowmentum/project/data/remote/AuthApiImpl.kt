package org.meowmentum.project.data.remote

import io.ktor.client.*
import io.ktor.client.call.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.*

class AuthApiImpl(
    private val client: HttpClient,
    private val baseUrl: String = "http://10.0.2.2:8080/api" // Replace with your actual API base URL
) : AuthApi {

    override suspend fun register(request: RegisterUserRequest): String {
        return client.post("$baseUrl/core/api/auth/register") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun verifyOtp(request: OtpValidationRequest): String {
        return client.post("$baseUrl/core/api/auth/verify-otp") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun login(request: LoginRequest): LoginResponse {
        return client.post("$baseUrl/core/api/auth/login") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun sendResetOtp(email: String): String {
        return client.post("$baseUrl/core/api/auth/send-reset-otp") {
            contentType(ContentType.Application.Json)
            setBody(PasswordResetRequest(email))
        }.body()
    }

    override suspend fun verifyResetOtp(request: OtpValidationRequest): ResetPasswordResponse {
        return client.post("$baseUrl/core/api/auth/verify-reset-otp") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun resetPassword(request: PasswordUpdateRequest): String {
        return client.post("$baseUrl/core/api/auth/reset-password") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun logout(): String {
        return client.post("$baseUrl/core/api/auth/logout").body()
    }
}