package org.meowmentum.project.ui.screens.auth.register

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.data.models.RegisterCredentials

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
    }

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

    fun register() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true)

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

            try {
                val result = authRepository.register(
                    RegisterCredentials(
                        name = _state.value.name,
                        email = _state.value.email,
                        password = _state.value.password
                    )
                )
                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isRegistered = true
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

    data class RegisterState(
        val name: String = "",
        val email: String = "",
        val password: String = "",
        val nameError: String? = null,
        val emailError: String? = null,
        val passwordError: String? = null,
        val isLoading: Boolean = false,
        val isRegistered: Boolean = false
    )
}