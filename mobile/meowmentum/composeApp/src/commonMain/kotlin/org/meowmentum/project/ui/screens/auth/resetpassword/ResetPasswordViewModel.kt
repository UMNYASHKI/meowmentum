package org.meowmentum.project.ui.screens.auth.resetpassword

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation
import org.meowmentum.project.domain.validation.ValidationResult

class ResetPasswordViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {
    private val _state = MutableStateFlow(ResetPasswordState())
    val state = _state.asStateFlow()

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage = _errorMessage.asStateFlow()

    fun setToken(token: String) {
        _state.value = _state.value.copy(token = token)
    }

    fun onNewPasswordChanged(password: String) {
        _state.value = _state.value.copy(
            newPassword = password,
            newPasswordError = null,
            confirmPasswordError = null
        )
        _errorMessage.value = null
    }

    fun onConfirmPasswordChanged(password: String) {
        _state.value = _state.value.copy(
            confirmPassword = password,
            confirmPasswordError = null
        )
        _errorMessage.value = null
    }

    fun resetPassword() {
        viewModelScope.launch {
            try {
                _errorMessage.value = null
                _state.value = _state.value.copy(isLoading = true)

                // Validate new password
                val passwordValidation = AuthValidation.validatePassword(_state.value.newPassword)
                if (passwordValidation is ValidationResult.Invalid) {
                    _state.value = _state.value.copy(
                        newPasswordError = passwordValidation.message,
                        isLoading = false
                    )
                    return@launch
                }

                // Validate password confirmation
                if (_state.value.newPassword != _state.value.confirmPassword) {
                    _state.value = _state.value.copy(
                        confirmPasswordError = "Passwords do not match",
                        isLoading = false
                    )
                    return@launch
                }

                val result = authRepository.resetPassword(
                    email = "", // Add email if required by your API
                    token = _state.value.token,
                    newPassword = _state.value.newPassword
                )

                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isPasswordReset = true
                        )
                    },
                    onFailure = { error ->
                        _errorMessage.value = when {
                            error.message?.contains("token", ignoreCase = true) == true ->
                                "Reset token has expired. Please try again"
                            error.message?.contains("network", ignoreCase = true) == true ->
                                "Network error. Please check your connection"
                            else -> error.message ?: "Failed to reset password"
                        }
                        _state.value = _state.value.copy(isLoading = false)
                    }
                )
            } catch (e: Exception) {
                _errorMessage.value = "An unexpected error occurred"
                _state.value = _state.value.copy(isLoading = false)
            }
        }
    }

    data class ResetPasswordState(
        val token: String = "",
        val newPassword: String = "",
        val confirmPassword: String = "",
        val newPasswordError: String? = null,
        val confirmPasswordError: String? = null,
        val isLoading: Boolean = false,
        val isPasswordReset: Boolean = false
    )
}