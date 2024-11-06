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
    val authManager = koinInject<AuthRepository>()
    val isLoggedIn by authManager.isUserLoggedIn().collectAsState(initial = false)

    Navigator(
        screen = if (isLoggedIn) HomeScreen() else LoginScreen(),
        disposeBehavior = NavigatorDisposeBehavior(
            disposeNestedNavigators = false,
            disposeSteps = true
        )
    ) { navigator ->
        SlideTransition(navigator)
    }
}

sealed class Screen {
    data class Auth(val route: AuthRoute) : Screen()
    object Home : Screen()
}

sealed class AuthRoute {
    object Login : AuthRoute()
    object Register : AuthRoute()
    object ForgotPassword : AuthRoute()
    data class ResetPassword(val token: String) : AuthRoute()
}