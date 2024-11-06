package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
import cafe.adriel.voyager.navigator.Navigator
import cafe.adriel.voyager.transitions.SlideTransition
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.auth.register.RegisterScreen

sealed class Screen {
    object Login : Screen()
    object Register : Screen()
    object Home : Screen()
    object ForgotPassword : Screen()
    object ResetPassword : Screen()
}

@Composable
fun AppNavigation() {
    Navigator(screen = LoginScreen()) {
        SlideTransition(it)
    }
}