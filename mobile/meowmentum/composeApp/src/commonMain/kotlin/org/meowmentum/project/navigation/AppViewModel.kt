package org.meowmentum.project.viewmodel

import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.flow.collectLatest
import kotlinx.coroutines.flow.distinctUntilChanged
import kotlinx.coroutines.flow.receiveAsFlow
import kotlinx.coroutines.launch
import org.meowmentum.project.domain.repository.AuthRepository

class AppViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {

    val isLoggedIn: Boolean = false
    val isLoading: Boolean = true

    sealed interface LoginState {
        data object Loading : LoginState
        data object LoggedIn : LoginState
        data object LoggedOut : LoginState
    }

    private val _sideEffects = Channel<LoginState>(Channel.BUFFERED)
    val sideEffects = _sideEffects.receiveAsFlow().distinctUntilChanged()

    init {
        viewModelScope.launch {
            authRepository.isUserLoggedIn().collectLatest { isLoggedIn ->
                _sideEffects.send(
                    if (isLoggedIn) {
                        LoginState.LoggedIn
                    } else {
                        LoginState.LoggedOut
                    }
                )
            }
        }
    }
}