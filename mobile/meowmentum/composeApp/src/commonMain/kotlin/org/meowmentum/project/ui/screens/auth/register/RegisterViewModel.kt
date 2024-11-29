package org.meowmentum.project.ui.screens.auth.register

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation
import org.meowmentum.project.domain.validation.ValidationResult

class RegisterViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {
    private val _state = MutableStateFlow(RegisterState())
    val state = _state.asStateFlow()

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage = _errorMessage.asStateFlow()

    fun onNameChanged(name: String) {
        _state.value = _state.value.copy(
            name = name,
            nameError = null
        )
        _errorMessage.value = null
    }

    fun onEmailChanged(email: String) {
        _state.value = _state.value.copy(
            email = email,
            emailError = null
        )
        _errorMessage.value = null
    }

    fun onPasswordChanged(password: String) {
        _state.value = _state.value.copy(
            password = password,
            passwordError = null
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

    fun register() {
        viewModelScope.launch {
            try {
                _errorMessage.value = null
                _state.value = _state.value.copy(isLoading = true)

                // Validate inputs
                val nameValidation = AuthValidation.validateName(_state.value.name)
                val emailValidation = AuthValidation.validateEmail(_state.value.email)
                val passwordValidation = AuthValidation.validatePassword(_state.value.password)

                if (nameValidation is ValidationResult.Invalid ||
                    emailValidation is ValidationResult.Invalid ||
                    passwordValidation is ValidationResult.Invalid) {
                    _state.value = _state.value.copy(
                        nameError = (nameValidation as? ValidationResult.Invalid)?.message,
                        emailError = (emailValidation as? ValidationResult.Invalid)?.message,
                        passwordError = (passwordValidation as? ValidationResult.Invalid)?.message,
                        isLoading = false
                    )
                    return@launch
                }

                val result = authRepository.register(
                    email = _state.value.email,
                    password = _state.value.password,
                    name = _state.value.name
                )

                result.fold(
                    onSuccess = { message ->
                        _state.value = _state.value.copy(
                            isLoading = false,
                            showVerificationField = true
                        )
                        _errorMessage.value = null
                    },
                    onFailure = { error ->
                        _errorMessage.value = when {
                            error.message?.contains("email", ignoreCase = true) == true ->
                                "This email is already registered"
                            error.message?.contains("network", ignoreCase = true) == true ->
                                "Network error. Please check your connection"
                            else -> error.message ?: "Registration failed"
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

                val result = authRepository.verifyOtp(
                    email = _state.value.email,
                    code = _state.value.verificationCode
                )

                result.fold(
                    onSuccess = { message ->
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isVerified = true
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

    data class RegisterState(
        val name: String = "",
        val email: String = "",
        val password: String = "",
        val verificationCode: String = "",
        val nameError: String? = null,
        val emailError: String? = null,
        val passwordError: String? = null,
        val verificationError: String? = null,
        val isLoading: Boolean = false,
        val showVerificationField: Boolean = false,
        val isVerified: Boolean = false
    )
}