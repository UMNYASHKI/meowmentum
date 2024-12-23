package org.meowmentum.project.ui.screens.auth.login

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation
import org.meowmentum.project.domain.validation.ValidationResult

class LoginViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {
    private val _state = MutableStateFlow(LoginState())
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

    fun onPasswordChanged(password: String) {
        _state.value = _state.value.copy(
            password = password,
            passwordError = null
        )
        _errorMessage.value = null
    }

    fun login() {
        viewModelScope.launch {
            try {
                _errorMessage.value = null

                val emailValidation = AuthValidation.validateEmail(_state.value.email)
                val passwordValidation = AuthValidation.validatePassword(_state.value.password)

                if (emailValidation is ValidationResult.Invalid ||
                    passwordValidation is ValidationResult.Invalid
                ) {
                    _state.value = _state.value.copy(
                        emailError = (emailValidation as? ValidationResult.Invalid)?.message,
                        passwordError = (passwordValidation as? ValidationResult.Invalid)?.message,
                        isLoading = false
                    )
                    return@launch
                }

                _state.value = _state.value.copy(isLoading = true)

                val result = authRepository.login(
                    email = _state.value.email.trim(),
                    password = _state.value.password
                )

                result.fold(
                    onSuccess = { token ->
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isLoggedIn = true
                        )
                    },
                    onFailure = { error ->
                        _errorMessage.value = when {
                            error.message?.contains("401", ignoreCase = true) == true ->
                                "Invalid email or password"
                            error.message?.contains("network", ignoreCase = true) == true ->
                                "Network error. Please check your connection"
                            else -> error.message ?: "Login failed"
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

    data class LoginState(
        val email: String = "",
        val password: String = "",
        val emailError: String? = null,
        val passwordError: String? = null,
        val isLoading: Boolean = false,
        val isLoggedIn: Boolean = false
    )
}