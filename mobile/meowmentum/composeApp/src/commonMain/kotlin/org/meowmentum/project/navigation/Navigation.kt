package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import cafe.adriel.voyager.navigator.Navigator
import cafe.adriel.voyager.transitions.SlideTransition
import org.koin.compose.koinInject
import org.koin.core.annotation.KoinInternalApi
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.home.HomeScreen
import org.meowmentum.project.ui.screens.task.list.TaskListScreen
import org.meowmentum.project.viewmodel.AppViewModel

@OptIn(KoinInternalApi::class)
@Composable
fun AppNavigation() {
    val authRepository: AuthRepository = koinInject()
    val viewModel: AppViewModel = koinInject()
    val isLoggedInFlow = authRepository.isUserLoggedIn()
    val sideEffect by viewModel.sideEffects.collectAsState(false)

    Navigator(
        screen = HomeScreen()
    ) { navigator ->
        SlideTransition(navigator)
        LaunchedEffect(sideEffect) {
            when (sideEffect) {
                AppViewModel.LoginState.LoggedIn -> {
                    navigator.push(TaskListScreen())
                    println("pushing to TaskListScreen")
                }
                AppViewModel.LoginState.LoggedOut -> {
                    navigator.push(LoginScreen())
                    println("pushing to LoginScreen")
                }
                else -> println("not pushing to TaskListScreen")
            }
        }
    }
}