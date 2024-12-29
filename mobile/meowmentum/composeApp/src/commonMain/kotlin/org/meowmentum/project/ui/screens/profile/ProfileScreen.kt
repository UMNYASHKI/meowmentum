package org.meowmentum.project.ui.screens.profile

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.meowmentum.project.ui.components.navigation.AppBottomNavigation
import org.meowmentum.project.ui.components.navigation.NavigationItem
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.task.list.TaskListScreen
import org.meowmentum.project.ui.screens.timer.TimerScreen

//TODO: Fix ui
@OptIn(ExperimentalMaterial3Api::class)
class ProfileScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: ProfileViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        var showDeleteDialog by remember { mutableStateOf<Long?>(null) }
        val navigator = LocalNavigator.currentOrThrow

        Scaffold(
            topBar = {
                CenterAlignedTopAppBar(
                    title = { Text("Profile") },
                    colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                        containerColor = MaterialTheme.colorScheme.background,
                        titleContentColor = MaterialTheme.colorScheme.primary
                    )
                )
            },
            bottomBar = {
                AppBottomNavigation(
                    currentRoute = NavigationItem.PROFILE.route,
                    onNavigate = {
                        when (it) {
                            NavigationItem.TASKS -> navigator.push(TaskListScreen())
                            NavigationItem.TIMER -> navigator.push(TimerScreen())
                            NavigationItem.PROFILE -> { /* Already on profile */ }
                        }
                    }
                )
            }
        ) { padding ->
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
                    .padding(16.dp),
                verticalArrangement = Arrangement.spacedBy(16.dp)
            ) {
                // User Info Card
                OutlinedCard(
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Column(
                        modifier = Modifier.padding(16.dp),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        //TODO: Replace with real user data
                        Text("Email: ${state.user?.email}", style = MaterialTheme.typography.bodyMedium)
                        Button(
                            onClick = { viewModel.logout() },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = MaterialTheme.colorScheme.error
                            ),
                            modifier = Modifier.fillMaxWidth(),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Text("Logout")
                        }
                    }
                }

                // Tags Management
                Text("Tags", style = MaterialTheme.typography.titleLarge)

                // New Tag Input
                Column(
                    modifier = Modifier,
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    OutlinedTextField(
                        value = state.newTagName,
                        onValueChange = viewModel::updateNewTagName,
                        label = { Text("New Tag") },
                        modifier = Modifier.fillMaxWidth(),
                        textStyle = MaterialTheme.typography.bodyMedium,
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = MaterialTheme.colorScheme.primary,
                            unfocusedBorderColor = MaterialTheme.colorScheme.tertiary,
                            focusedLabelColor = MaterialTheme.colorScheme.primary,
                            unfocusedLabelColor = MaterialTheme.colorScheme.secondary,
                            errorBorderColor = MaterialTheme.colorScheme.error,
                            errorLabelColor = MaterialTheme.colorScheme.error,
                            errorSupportingTextColor = MaterialTheme.colorScheme.error
                        ),
                        shape = RoundedCornerShape(12.dp),
                        singleLine = true
                    )
                    Button(
                        onClick = {
                            if (state.editingTag != null) {
                                viewModel.updateTag()
                            } else {
                                viewModel.createTag()
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
                        Text(if (state.editingTag != null) "Update" else "Add")
                    }
                }

                // Tags List
                state.tags.forEach { tag ->
                    Card(
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Row(
                            modifier = Modifier.padding(16.dp),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Text(tag.name, style = MaterialTheme.typography.bodyLarge, modifier = Modifier.weight(1f))
                            Row {
                                IconButton(
                                    onClick = { viewModel.startEditingTag(tag) }
                                ) {
                                    Icon(Icons.Default.Edit, "Edit Tag")
                                }
                                IconButton(
                                    onClick = { showDeleteDialog = tag.id }
                                ) {
                                    Icon(Icons.Default.Delete, "Delete Tag")
                                }
                            }
                        }
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

            showDeleteDialog?.let { tagId ->
                AlertDialog(
                    onDismissRequest = { showDeleteDialog = null },
                    title = { Text("Delete Tag") },
                    text = { Text("Are you sure you want to delete this tag?") },
                    confirmButton = {
                        TextButton(
                            onClick = {
                                viewModel.deleteTag(tagId)
                                showDeleteDialog = null
                            }
                        ) {
                            Text("Delete")
                        }
                    },
                    dismissButton = {
                        TextButton(onClick = { showDeleteDialog = null }) {
                            Text("Cancel")
                        }
                    }
                )
            }
        }
    }
}
