package org.meowmentum.project.ui.screens.home

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.Logout
import androidx.compose.material.icons.filled.Logout
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import org.koin.compose.koinInject
import org.meowmentum.project.domain.repository.AuthRepository

class HomeScreen : Screen {
    @Composable
    override fun Content() {
        val authRepository: AuthRepository = koinInject()
        val currentUser by authRepository.getCurrentUser().collectAsState(initial = null)
        val scope = rememberCoroutineScope()

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp)
        ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    text = "Welcome, ${currentUser?.name ?: "User"}",
                    style = MaterialTheme.typography.headlineMedium
                )

                IconButton(
                    onClick = {
                        scope.launch {
                            authRepository.logout()
                        }
                    }
                ) {
                    Icon(Icons.AutoMirrored.Filled.Logout, contentDescription = "Logout")
                }
            }
        }
    }
}