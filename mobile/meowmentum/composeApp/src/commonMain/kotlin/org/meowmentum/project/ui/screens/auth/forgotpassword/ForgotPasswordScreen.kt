package org.meowmentum.project.ui.screens.auth.forgotpassword

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
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

class ForgotPasswordScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: ForgotPasswordViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        LaunchedEffect(state.isEmailSent) {
            if (state.isEmailSent) {
                // Show success message and navigate back
                navigator.pop()
            }
        }

        Box(modifier = Modifier.fillMaxSize()) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(16.dp),
                verticalArrangement = Arrangement.spacedBy(16.dp)
            ) {
                IconButton(
                    onClick = { navigator.pop() }
                ) {
                    Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                }

                Text(
                    text = "Forgot password?",
                    style = MaterialTheme.typography.headlineMedium
                )

                Text(
                    text = "Enter the email associated with your account and we'll send you an email with instructions to reset your password.",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )

                errorMessage?.let { ErrorCard(message = it) }

                CommonTextField(
                    value = state.email,
                    onValueChange = viewModel::onEmailChanged,
                    label = "Email",
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    isError = state.emailError != null,
                    errorMessage = state.emailError
                )

                PrimaryButton(
                    text = "Send Instructions",
                    onClick = viewModel::sendResetEmail,
                    enabled = !state.isLoading
                )
            }

            if (state.isLoading) {
                LoadingOverlay()
            }
        }
    }
}