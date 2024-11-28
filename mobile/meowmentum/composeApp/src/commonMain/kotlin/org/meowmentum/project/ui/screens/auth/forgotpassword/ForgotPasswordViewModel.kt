package org.meowmentum.project.ui.screens.auth.forgotpassword

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation
import org.meowmentum.project.domain.validation.ValidationResult

class ForgotPasswordViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {
    private val _state = MutableStateFlow(ForgotPasswordState())
    val state = _state.asStateFlow()

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage = _errorMessage.asStateFlow()

    fun onEmailChanged(email: String) {
        _state.value = _state.value.copy(
            email = email,
            emailError = null
        )
        _errorMessage.value = null
    }

    fun onVerificationCodeChanged(code: String) {
        if (code.length <= 6) {
            _state.value = _state.value.copy(
                verificationCode = code,
                verificationError = null
            )
            _errorMessage.value = null
        }
    }

    fun sendResetEmail() {
        viewModelScope.launch {
            try {
                _errorMessage.value = null

                val emailValidation = AuthValidation.validateEmail(_state.value.email)
                if (emailValidation is ValidationResult.Invalid) {
                    _state.value = _state.value.copy(
                        emailError = emailValidation.message
                    )
                    return@launch
                }

                _state.value = _state.value.copy(isLoading = true)

                val result = authRepository.sendResetOtp(_state.value.email)

                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            showVerificationField = true
                        )
                        _errorMessage.value = null
                    },
                    onFailure = { error ->
                        _errorMessage.value = when {
                            error.message?.contains("not found", ignoreCase = true) == true ->
                                "No account found with this email"
                            error.message?.contains("network", ignoreCase = true) == true ->
                                "Network error. Please check your connection"
                            else -> error.message ?: "Failed to send verification code"
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

    fun verifyCode() {
        viewModelScope.launch {
            try {
                if (_state.value.verificationCode.length != 6) {
                    _state.value = _state.value.copy(
                        verificationError = "Please enter a valid 6-digit code"
                    )
                    return@launch
                }

                _state.value = _state.value.copy(isLoading = true)
                _errorMessage.value = null

                val result = authRepository.verifyResetOtp(
                    email = _state.value.email,
                    code = _state.value.verificationCode
                )

                result.fold(
                    onSuccess = { token ->
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isVerified = true,
                            resetToken = token
                        )
                    },
                    onFailure = { error ->
                        _errorMessage.value = when {
                            error.message?.contains("invalid", ignoreCase = true) == true ->
                                "Invalid verification code"
                            error.message?.contains("expired", ignoreCase = true) == true ->
                                "Verification code has expired"
                            error.message?.contains("network", ignoreCase = true) == true ->
                                "Network error. Please check your connection"
                            else -> error.message ?: "Verification failed"
                        }
                        _state.value = _state.value.copy(
                            isLoading = false,
                            verificationError = "Invalid code"
                        )
                    }
                )
            } catch (e: Exception) {
                _errorMessage.value = "An unexpected error occurred"
                _state.value = _state.value.copy(isLoading = false)
            }
        }
    }

    data class ForgotPasswordState(
        val email: String = "",
        val verificationCode: String = "",
        val emailError: String? = null,
        val verificationError: String? = null,
        val isLoading: Boolean = false,
        val showVerificationField: Boolean = false,
        val isVerified: Boolean = false,
        val resetToken: String = ""
    )
}