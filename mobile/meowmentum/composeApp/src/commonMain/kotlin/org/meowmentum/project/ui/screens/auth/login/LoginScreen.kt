package org.meowmentum.project.ui.screens.auth.login

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.meowmentum.project.ui.components.*
import org.meowmentum.project.ui.screens.auth.register.RegisterScreen
import org.meowmentum.project.ui.screens.home.HomeScreen

class LoginScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: LoginViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        LaunchedEffect(state.isLoggedIn) {
            if (state.isLoggedIn) {
                navigator.replace(HomeScreen())
            }
        }

        LoginContent(
            state = state,
            errorMessage = errorMessage,
            onEmailChange = viewModel::onEmailChanged,
            onPasswordChange = viewModel::onPasswordChanged,
            onLoginClick = viewModel::login,
            onGoogleLoginClick = viewModel::loginWithGoogle,
            onForgotPasswordClick = { /* Navigate to Forgot Password */ },
            onSignUpClick = { navigator.push(RegisterScreen()) }
        )
    }
}

@Composable
private fun LoginContent(
    state: LoginViewModel.LoginState,
    errorMessage: String?,
    onEmailChange: (String) -> Unit,
    onPasswordChange: (String) -> Unit,
    onLoginClick: () -> Unit,
    onGoogleLoginClick: (String) -> Unit,
    onForgotPasswordClick: () -> Unit,
    onSignUpClick: () -> Unit
) {
    Box(modifier = Modifier.fillMaxSize()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Text(
                text = "Log in",
                style = MaterialTheme.typography.headlineMedium
            )

            if (errorMessage != null) {
                ErrorCard(message = errorMessage)
            }

            CommonTextField(
                value = state.email,
                onValueChange = onEmailChange,
                label = "Email",
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                isError = state.emailError != null,
                errorMessage = state.emailError
            )

            CommonTextField(
                value = state.password,
                onValueChange = onPasswordChange,
                label = "Password",
                isPassword = true,
                isError = state.passwordError != null,
                errorMessage = state.passwordError
            )

            PrimaryButton(
                text = "Log In",
                onClick = onLoginClick,
                enabled = !state.isLoading
            )

            GoogleButton(
                onClick = { /* Implement Google Sign In */ },
                //enabled = !state.isLoading
            )

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                TextButton(
                    onClick = onForgotPasswordClick,
                    enabled = !state.isLoading
                ) {
                    Text("Forgot Password?")
                }
                TextButton(
                    onClick = onSignUpClick,
                    enabled = !state.isLoading
                ) {
                    Text("Sign Up")
                }
            }
        }

        if (state.isLoading) {
            LoadingOverlay()
        }
    }
}