package org.meowmentum.project.data.local

interface AuthTokenStorage {
    suspend fun saveTokens(token: String, refreshToken: String)
    suspend fun getToken(): String?
    suspend fun getRefreshToken(): String?
    suspend fun clearTokens()
}