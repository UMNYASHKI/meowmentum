package org.meowmentum.project.data.local

import kotlinx.coroutines.flow.Flow

interface AuthTokenStorage {
    suspend fun saveTokens(token: String, refreshToken: String)
    suspend fun getToken(): String?
    suspend fun getRefreshToken(): String?
    fun getTokenFlow(): Flow<String?>
    fun getRefreshTokenFlow(): Flow<String?>
    suspend fun clearTokens()
}

expect fun createAuthTokenStorage(): AuthTokenStorage