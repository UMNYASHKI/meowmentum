package org.meowmentum.project.data.local

import kotlinx.coroutines.flow.Flow

interface AuthTokenStorage {
    suspend fun saveTokens(accessToken: String, refreshToken: String)
    suspend fun getAccessToken(): String?
    suspend fun getRefreshToken(): String?
    fun getAccessTokenFlow(): Flow<String?>
    fun getRefreshTokenFlow(): Flow<String?>
    suspend fun clearTokens()
    suspend fun updateAccessToken(newToken: String)
}
