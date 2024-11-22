package org.meowmentum.project.ui.screens.auth.login

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.meowmentum.project.ui.components.*
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordScreen
import org.meowmentum.project.ui.screens.auth.register.RegisterScreen

class LoginScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: LoginViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

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

                errorMessage?.let { ErrorCard(message = it) }

                CommonTextField(
                    value = state.email,
                    onValueChange = viewModel::onEmailChanged,
                    label = "Email",
                    isError = state.emailError != null,
                    errorMessage = state.emailError
                )

                CommonTextField(
                    value = state.password,
                    onValueChange = viewModel::onPasswordChanged,
                    label = "Password",
                    isPassword = true,
                    isError = state.passwordError != null,
                    errorMessage = state.passwordError
                )

                Button(
                    onClick = viewModel::login,
                    enabled = !state.isLoading,
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Text("Log In")
                }

                Button(
                    onClick = { /* Implement Google Sign In */ },
                    enabled = !state.isLoading,
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Text("Continue with Google")
                }

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    TextButton(
                        onClick = { navigator.push(ForgotPasswordScreen()) }
                    ) {
                        Text("Forgot Password?")
                    }
                    TextButton(
                        onClick = { navigator.push(RegisterScreen()) }
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
}