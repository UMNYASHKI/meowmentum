package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
import cafe.adriel.voyager.core.screen.Screen
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordScreen
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.auth.register.RegisterScreen
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordScreen

sealed class AppScreen {
    data class Auth(val screen: AuthScreen) : AppScreen()
    object Home : AppScreen()
}

sealed class AuthScreen : Screen {
    object Login : AuthScreen()
    object Register : AuthScreen()
    object ForgotPassword : AuthScreen()
    data class ResetPassword(val token: String) : AuthScreen() {
        @Composable
        override fun Content() {
            ResetPasswordScreen(token).Content()
        }
    }

    // Each auth screen needs to implement Content()
    @Composable
    override fun Content() {
        when (this) {
            is Login -> LoginScreen().Content()
            is Register -> RegisterScreen().Content()
            is ForgotPassword -> ForgotPasswordScreen().Content()
            is ResetPassword -> ResetPasswordScreen(token).Content()
        }
    }
}