package org.meowmentum.project.domain.repository

import kotlinx.coroutines.flow.Flow
import org.meowmentum.project.data.models.LoginCredentials
import org.meowmentum.project.data.models.RegisterCredentials
import org.meowmentum.project.domain.model.User

interface AuthRepository {
    suspend fun login(credentials: LoginCredentials): Result<User>
    suspend fun register(credentials: RegisterCredentials): Result<User>
    suspend fun loginWithGoogle(token: String): Result<User>
    suspend fun logout()
    suspend fun sendPasswordResetEmail(email: String): Result<Unit>
    suspend fun resetPassword(token: String, newPassword: String): Result<Unit>
    fun isUserLoggedIn(): Flow<Boolean>
}