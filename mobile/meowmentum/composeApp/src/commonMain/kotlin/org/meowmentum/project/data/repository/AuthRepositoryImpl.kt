package org.meowmentum.project.data.repository

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.map
import org.meowmentum.project.data.local.AuthTokenStorage
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.auth.AuthApi
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.domain.model.User
import org.meowmentum.project.domain.repository.AuthRepository

class AuthRepositoryImpl(
    private val api: AuthApi,
    private val tokenStorage: AuthTokenStorage
) : AuthRepository {
    private val _currentUser = MutableStateFlow<User?>(null)

    override suspend fun register(email: String, password: String, name: String): Result<String> {
        return when (val result = api.register(RegisterUserRequest(email, password, name))) {
            is ApiResult.Success -> Result.success(result.data)
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun verifyOtp(email: String, code: String): Result<String> {
        return when (val result = api.verifyOtp(OtpValidationRequest(email, code))) {
            is ApiResult.Success -> Result.success(result.data)
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun login(email: String, password: String): Result<String> {
        return when (val result = api.login(LoginRequest(email, password))) {
            is ApiResult.Success -> {
                tokenStorage.saveTokens(accessToken = result.data.token, refreshToken = "")
                Result.success(result.data.token)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun sendResetOtp(email: String): Result<String> {
        return when (val result = api.sendResetOtp(PasswordResetRequest(email))) {
            is ApiResult.Success -> Result.success(result.data)
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun verifyResetOtp(email: String, code: String): Result<String> {
        return when (val result = api.verifyResetOtp(OtpValidationRequest(email, code))) {
            is ApiResult.Success -> Result.success(result.data.token)
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun resetPassword(email: String, token: String, newPassword: String): Result<String> {
        return when (val result = api.resetPassword(PasswordUpdateRequest(email, token, newPassword))) {
            is ApiResult.Success -> Result.success(result.data)
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun logout(): Result<String> {
        return when (val result = api.logout()) {
            is ApiResult.Success -> {
                tokenStorage.clearTokens()
                _currentUser.value = null
                Result.success(result.data)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override fun getCurrentUser(): Flow<User?> = _currentUser
    override fun isUserLoggedIn(): Flow<Boolean> = tokenStorage.getAccessTokenFlow().map { !it.isNullOrEmpty() }
}