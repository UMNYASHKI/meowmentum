package org.meowmentum.project.ui.screens.auth.forgotpassword

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import org.meowmentum.project.data.models.ValidationResult
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.validation.AuthValidation

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
    }

    fun sendResetEmail() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true)

            val emailValidation = AuthValidation.validateEmail(_state.value.email)
            if (emailValidation is ValidationResult.Invalid) {
                _state.value = _state.value.copy(
                    emailError = emailValidation.message,
                    isLoading = false
                )
                return@launch
            }

            try {
                val result = authRepository.sendPasswordResetEmail(_state.value.email)
                result.fold(
                    onSuccess = {
                        _state.value = _state.value.copy(
                            isLoading = false,
                            isEmailSent = true
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

    data class ForgotPasswordState(
        val email: String = "",
        val emailError: String? = null,
        val isLoading: Boolean = false,
        val isEmailSent: Boolean = false
    )
}