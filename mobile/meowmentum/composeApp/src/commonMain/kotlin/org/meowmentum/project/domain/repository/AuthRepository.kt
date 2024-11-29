package org.meowmentum.project.domain.repository

import kotlinx.coroutines.flow.Flow
import org.meowmentum.project.data.models.*
import org.meowmentum.project.domain.model.User

interface AuthRepository {
    suspend fun register(email: String, password: String, name: String): Result<String>
    suspend fun verifyOtp(email: String, code: String): Result<String>
    suspend fun login(email: String, password: String): Result<String>
    suspend fun sendResetOtp(email: String): Result<String>
    suspend fun verifyResetOtp(email: String, code: String): Result<String>
    suspend fun resetPassword(email: String, token: String, newPassword: String): Result<String>
    suspend fun logout(): Result<String>
    fun getCurrentUser(): Flow<User?>
    fun isUserLoggedIn(): Flow<Boolean>
}