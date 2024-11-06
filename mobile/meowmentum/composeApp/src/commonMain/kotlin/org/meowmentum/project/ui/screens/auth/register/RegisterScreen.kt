package org.meowmentum.project.ui.screens.auth.register

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
import org.meowmentum.project.ui.screens.home.HomeScreen

class RegisterScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: RegisterViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        LaunchedEffect(state.isRegistered) {
            if (state.isRegistered) {
                navigator.replace(HomeScreen())
            }
        }

        RegisterContent(
            state = state,
            errorMessage = errorMessage,
            onNameChange = viewModel::onNameChanged,
            onEmailChange = viewModel::onEmailChanged,
            onPasswordChange = viewModel::onPasswordChanged,
            onRegisterClick = viewModel::register,
            onGoogleRegisterClick = { /* Implement Google Sign In */ },
            onLoginClick = { navigator.pop() }
        )
    }
}

@Composable
private fun RegisterContent(
    state: RegisterViewModel.RegisterState,
    errorMessage: String?,
    onNameChange: (String) -> Unit,
    onEmailChange: (String) -> Unit,
    onPasswordChange: (String) -> Unit,
    onRegisterClick: () -> Unit,
    onGoogleRegisterClick: () -> Unit,
    onLoginClick: () -> Unit
) {
    Box(modifier = Modifier.fillMaxSize()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Text(
                text = "Create an account",
                style = MaterialTheme.typography.headlineMedium
            )

            if (errorMessage != null) {
                ErrorCard(message = errorMessage)
            }

            CommonTextField(
                value = state.name,
                onValueChange = onNameChange,
                label = "Name",
                isError = state.nameError != null,
                errorMessage = state.nameError
            )

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
                supportingText = "Must be at least 8 characters.",
                isError = state.passwordError != null,
                errorMessage = state.passwordError
            )

            PrimaryButton(
                text = "Sign Up",
                onClick = onRegisterClick,
                enabled = !state.isLoading
            )

            GoogleButton(
                onClick = onGoogleRegisterClick,
                //enabled = !state.isLoading
            )

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.Center
            ) {
                Text("Already have an account? ")
                TextButton(
                    onClick = onLoginClick,
                    enabled = !state.isLoading
                ) {
                    Text("Log in")
                }
            }
        }

        if (state.isLoading) {
            LoadingOverlay()
        }
    }
}