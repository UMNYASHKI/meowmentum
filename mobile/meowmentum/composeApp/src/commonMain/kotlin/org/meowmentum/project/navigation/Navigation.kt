package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import cafe.adriel.voyager.navigator.Navigator
import cafe.adriel.voyager.navigator.NavigatorDisposeBehavior
import cafe.adriel.voyager.transitions.SlideTransition
import org.koin.compose.koinInject
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.home.HomeScreen

@Composable
fun AppNavigation() {
    val authRepository: AuthRepository = koinInject()
    val isLoggedIn by authRepository.isUserLoggedIn().collectAsState(initial = false)

    Navigator(
        screen = if (isLoggedIn) HomeScreen() else LoginScreen()
    ) { navigator ->
        SlideTransition(navigator)
    }
}