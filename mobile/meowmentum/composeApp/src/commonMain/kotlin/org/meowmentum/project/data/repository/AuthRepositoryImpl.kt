package org.meowmentum.project.data.repository

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.map
import org.meowmentum.project.data.local.AuthTokenStorage
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.AuthApi
import org.meowmentum.project.domain.model.User
import org.meowmentum.project.domain.model.toDomain
import org.meowmentum.project.domain.repository.AuthRepository

class AuthRepositoryImpl(
    private val api: AuthApi,
    private val tokenStorage: AuthTokenStorage
) : AuthRepository {
    private val _currentUser = MutableStateFlow<UserDto?>(null)

    override suspend fun login(credentials: LoginCredentials): Result<User> {
        return try {
            val response = api.login(credentials)
            handleAuthResponse(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun register(credentials: RegisterCredentials): Result<User> {
        return try {
            val response = api.register(credentials)
            handleAuthResponse(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun loginWithGoogle(token: String): Result<User> {
        return try {
            val response = api.loginWithGoogle(token)
            handleAuthResponse(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun logout() {
        tokenStorage.clearTokens()
        _currentUser.value = null
    }

    override suspend fun sendPasswordResetEmail(email: String): Result<Unit> {
        return try {
            api.sendPasswordResetEmail(email)
            Result.success(Unit)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun resetPassword(token: String, newPassword: String): Result<Unit> {
        return try {
            api.resetPassword(token, newPassword)
            Result.success(Unit)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override fun getCurrentUser(): Flow<User?> {
        return _currentUser.map { it?.toDomain() }
    }

    override fun isUserLoggedIn(): Flow<Boolean> {
        return _currentUser.map { it != null }
    }

    override suspend fun refreshToken(): Result<Unit> {
        return try {
            val refreshToken = tokenStorage.getRefreshToken() ?: return Result.failure(Exception("No refresh token"))
            val response = api.refreshToken(refreshToken)
            tokenStorage.saveTokens(response.token, response.refreshToken)
            Result.success(Unit)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    private suspend fun handleAuthResponse(response: AuthResponse): Result<User> {
        tokenStorage.saveTokens(response.token, response.refreshToken)
        _currentUser.value = response.user
        return Result.success(response.user.toDomain())
    }
}