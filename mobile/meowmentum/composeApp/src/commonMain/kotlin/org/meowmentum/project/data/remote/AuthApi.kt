package org.meowmentum.project.data.remote

import org.meowmentum.project.data.models.*

interface AuthApi {
    suspend fun login(credentials: LoginCredentials): AuthResponse
    suspend fun register(credentials: RegisterCredentials): AuthResponse
    suspend fun loginWithGoogle(token: String): AuthResponse
    suspend fun refreshToken(refreshToken: String): AuthResponse
    suspend fun sendPasswordResetEmail(email: String)
    suspend fun resetPassword(token: String, newPassword: String)
}
