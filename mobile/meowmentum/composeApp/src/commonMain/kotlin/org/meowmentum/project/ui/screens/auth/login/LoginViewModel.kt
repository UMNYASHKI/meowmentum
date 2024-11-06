package org.meowmentum.project.ui.screens.auth.login

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import org.meowmentum.project.data.models.LoginCredentials
import org.meowmentum.project.data.models.ValidationResult
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation

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
    }

    fun onPasswordChanged(password: String) {
        _state.value = _state.value.copy(
            password = password,
            passwordError = null
        )
    }

    fun login() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true)

            val emailValidation = AuthValidation.validateEmail(_state.value.email)
            val passwordValidation = AuthValidation.validatePassword(_state.value.password)

            if (emailValidation is ValidationResult.Invalid ||
                passwordValidation is ValidationResult.Invalid) {
                _state.value = _state.value.copy(
                    emailError = (emailValidation as? ValidationResult.Invalid)?.message,
                    passwordError = (passwordValidation as? ValidationResult.Invalid)?.message,
                    isLoading = false
                )
                return@launch
            }

            try {
                val result = authRepository.login(
                    LoginCredentials(
                        email = _state.value.email,
                        password = _state.value.password
                    )
                )
                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isLoggedIn = true
                        )
                    },
                    onFailure = { error ->
                        _errorMessage.value = error.message
                        _state.value = _state.value.copy(isLoading = false)
                    }
                )
            } catch (e: Exception) {
                _errorMessage.value = e.message
                _state.value = _state.value.copy(isLoading = false)
            }
        }
    }

    fun loginWithGoogle(token: String) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true)
            try {
                val result = authRepository.loginWithGoogle(token)
                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isLoggedIn = true
                        )
                    },
                    onFailure = { error ->
                        _errorMessage.value = error.message
                        _state.value = _state.value.copy(isLoading = false)
                    }
                )
            } catch (e: Exception) {
                _errorMessage.value = e.message
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