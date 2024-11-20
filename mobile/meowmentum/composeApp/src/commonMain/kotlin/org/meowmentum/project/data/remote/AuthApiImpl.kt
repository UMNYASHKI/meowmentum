package org.meowmentum.project.data.remote

import io.ktor.client.*
import io.ktor.client.call.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.*

class AuthApiImpl(
    private val client: HttpClient
) : AuthApi {
    private val baseUrl = "https://api.meowmentum.project" // Replace with your actual API base URL

    override suspend fun login(credentials: LoginCredentials): AuthResponse {
        return client.post("$baseUrl/auth/login") {
            contentType(ContentType.Application.Json)
            setBody(credentials)
        }.body()
    }

    override suspend fun register(credentials: RegisterCredentials): AuthResponse {
        return client.post("$baseUrl/auth/register") {
            contentType(ContentType.Application.Json)
            setBody(credentials)
        }.body()
    }

    override suspend fun loginWithGoogle(token: String): AuthResponse {
        return client.post("$baseUrl/auth/google") {
            contentType(ContentType.Application.Json)
            setBody(mapOf("token" to token))
        }.body()
    }

    override suspend fun refreshToken(refreshToken: String): AuthResponse {
        return client.post("$baseUrl/auth/refresh") {
            contentType(ContentType.Application.Json)
            setBody(mapOf("refreshToken" to refreshToken))
        }.body()
    }

    override suspend fun sendPasswordResetEmail(email: String) {
        client.post("$baseUrl/auth/forgot-password") {
            contentType(ContentType.Application.Json)
            setBody(mapOf("email" to email))
        }
    }

    override suspend fun resetPassword(token: String, newPassword: String) {
        client.post("$baseUrl/auth/reset-password") {
            contentType(ContentType.Application.Json)
            setBody(mapOf(
                "token" to token,
                "newPassword" to newPassword
            ))
        }
    }
}