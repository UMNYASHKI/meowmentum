// File: commonMain/kotlin/org/meowmentum/project/ui/screens/auth/forgotpassword/ForgotPasswordScreen.kt
package org.meowmentum.project.ui.screens.auth.forgotpassword

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordScreen

@OptIn(ExperimentalMaterial3Api::class)
class ForgotPasswordScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: ForgotPasswordViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val errorMessage by viewModel.errorMessage.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        LaunchedEffect(state.isVerified) {
            if (state.isVerified) {
                navigator.replace(ResetPasswordScreen(state.resetToken))
            }
        }

        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(MaterialTheme.colorScheme.background)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(horizontal = 24.dp)
                    .padding(top = 16.dp),
                verticalArrangement = Arrangement.spacedBy(24.dp)
            ) {
                IconButton(
                    onClick = { navigator.pop() },
                    colors = IconButtonDefaults.iconButtonColors(
                        contentColor = MaterialTheme.colorScheme.primary
                    )
                ) {
                    Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                }

                Text(
                    text = "Forgot password?",
                    style = MaterialTheme.typography.headlineLarge,
                    color = MaterialTheme.colorScheme.primary,
                    fontWeight = FontWeight.Bold
                )

                Text(
                    text = if (!state.showVerificationField) {
                        "Enter the email associated with your account and we'll send you an email with instructions to reset your password."
                    } else {
                        "Please enter the verification code we sent to your email."
                    },
                    style = MaterialTheme.typography.bodyLarge,
                    color = MaterialTheme.colorScheme.secondary
                )

                Column(
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    errorMessage?.let {
                        Text(
                            text = it,
                            color = MaterialTheme.colorScheme.error,
                            style = MaterialTheme.typography.bodySmall
                        )
                    }

                    OutlinedTextField(
                        value = state.email,
                        onValueChange = viewModel::onEmailChanged,
                        label = { Text("Email") },
                        enabled = !state.showVerificationField,
                        isError = state.emailError != null,
                        supportingText = state.emailError?.let { { Text(it) } },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                        modifier = Modifier.fillMaxWidth(),
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = MaterialTheme.colorScheme.primary,
                            unfocusedBorderColor = MaterialTheme.colorScheme.tertiary,
                            focusedLabelColor = MaterialTheme.colorScheme.primary,
                            unfocusedLabelColor = MaterialTheme.colorScheme.secondary,
                            errorBorderColor = MaterialTheme.colorScheme.error,
                            errorLabelColor = MaterialTheme.colorScheme.error,
                            disabledBorderColor = MaterialTheme.colorScheme.tertiary,
                            disabledTextColor = MaterialTheme.colorScheme.secondary
                        ),
                        shape = RoundedCornerShape(12.dp),
                        singleLine = true
                    )

                    if (state.showVerificationField) {
                        OutlinedTextField(
                            value = state.verificationCode,
                            onValueChange = viewModel::onVerificationCodeChanged,
                            label = { Text("Verification Code") },
                            isError = state.verificationError != null,
                            supportingText = state.verificationError?.let { { Text(it) } } ?:
                            { Text("Enter verification code from email you received") },
                            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                            modifier = Modifier.fillMaxWidth(),
                            colors = OutlinedTextFieldDefaults.colors(
                                focusedBorderColor = MaterialTheme.colorScheme.primary,
                                unfocusedBorderColor = MaterialTheme.colorScheme.tertiary,
                                focusedLabelColor = MaterialTheme.colorScheme.primary,
                                unfocusedLabelColor = MaterialTheme.colorScheme.secondary,
                                errorBorderColor = MaterialTheme.colorScheme.error,
                                errorLabelColor = MaterialTheme.colorScheme.error
                            ),
                            shape = RoundedCornerShape(12.dp),
                            singleLine = true
                        )
                    }

                    Button(
                        onClick = {
                            if (state.showVerificationField) {
                                viewModel.verifyCode()
                            } else {
                                viewModel.sendResetEmail()
                            }
                        },
                        enabled = !state.isLoading,
                        modifier = Modifier.fillMaxWidth(),
                        colors = ButtonDefaults.buttonColors(
                            containerColor = MaterialTheme.colorScheme.primary,
                            disabledContainerColor = MaterialTheme.colorScheme.tertiary
                        ),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Text(
                            if (state.showVerificationField) "Verify" else "Send Instructions",
                            modifier = Modifier.padding(vertical = 4.dp)
                        )
                    }
                }
            }

            if (state.isLoading) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .background(MaterialTheme.colorScheme.background.copy(alpha = 0.7f)),
                    contentAlignment = Alignment.Center
                ) {
                    CircularProgressIndicator(
                        color = MaterialTheme.colorScheme.primary
                    )
                }
            }
        }
    }
}