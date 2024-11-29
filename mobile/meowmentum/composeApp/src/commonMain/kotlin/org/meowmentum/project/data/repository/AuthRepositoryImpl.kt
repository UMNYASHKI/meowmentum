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

    override suspend fun register(email: String, password: String, name: String): Result<String> {
        return try {
            val response = api.register(RegisterUserRequest(email, password, name))
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun verifyOtp(email: String, code: String): Result<String> {
        return try {
            val response = api.verifyOtp(OtpValidationRequest(email, code))
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun login(email: String, password: String): Result<String> {
        return try {
            val response = api.login(LoginRequest(email, password))
            tokenStorage.saveTokens(
                accessToken = response.token,
                refreshToken = ""
            )
            Result.success(response.token)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun sendResetOtp(email: String): Result<String> {
        return try {
            val response = api.sendResetOtp(email)
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun verifyResetOtp(email: String, code: String): Result<String> {
        return try {
            val response = api.verifyResetOtp(OtpValidationRequest(email, code))
            Result.success(response.token)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun resetPassword(email: String, token: String, newPassword: String): Result<String> {
        return try {
            val response = api.resetPassword(PasswordUpdateRequest(email, token, newPassword))
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun logout(): Result<String> {
        return try {
            val response = api.logout()
            tokenStorage.clearTokens()
            _currentUser.value = null
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override fun getCurrentUser(): Flow<User?> = _currentUser.map { it?.toDomain() }

    override fun isUserLoggedIn(): Flow<Boolean> {
        // First check if we have a stored token
        return tokenStorage.getAccessTokenFlow().map { token ->
            token != null && token.isNotEmpty()
        }
    }}