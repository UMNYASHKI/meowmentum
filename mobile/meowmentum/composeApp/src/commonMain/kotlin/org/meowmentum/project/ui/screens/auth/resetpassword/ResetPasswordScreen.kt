package org.meowmentum.project.ui.screens.auth.resetpassword

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.meowmentum.project.ui.components.*

class ResetPasswordScreen(private val token: String) : Screen {
    @Composable
    override fun Content() {
        val viewModel: ResetPasswordViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        LaunchedEffect(Unit) {
            viewModel.setToken(token)
        }

        LaunchedEffect(state.isPasswordReset) {
            if (state.isPasswordReset) {
                // Navigate to login screen
                navigator.replaceAll(AuthScreen.Login())
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
                    text = "Create new password",
                    style = MaterialTheme.typography.headlineMedium
                )

                Text(
                    text = "This password should be different from the previous password.",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )

                if (errorMessage != null) {
                    ErrorCard(message = errorMessage)
                }

                CommonTextField(
                    value = state.newPassword,
                    onValueChange = viewModel::onNewPasswordChanged,
                    label = "New Password",
                    isPassword = true,
                    supportingText = "Must be at least 8 characters.",
                    isError = state.newPasswordError != null,
                    errorMessage = state.newPasswordError
                )

                CommonTextField(
                    value = state.confirmPassword,
                    onValueChange = viewModel::onConfirmPasswordChanged,
                    label = "Confirm Password",
                    isPassword = true,
                    supportingText = "Both passwords must match.",
                    isError = state.confirmPasswordError != null,
                    errorMessage = state.confirmPasswordError
                )

                PrimaryButton(
                    text = "Reset Password",
                    onClick = viewModel::resetPassword,
                    enabled = !state.isLoading
                )
            }

            if (state.isLoading) {
                LoadingOverlay()
            }
        }
    }
}