package org.meowmentum.project.ui.screens.task.edit

import android.annotation.SuppressLint
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import com.vanpra.composematerialdialogs.MaterialDialog
import com.vanpra.composematerialdialogs.datetime.date.datepicker
import com.vanpra.composematerialdialogs.rememberMaterialDialogState
import kotlinx.datetime.*
import org.koin.compose.koinInject
import org.meowmentum.project.ui.components.task.*
import org.threeten.bp.LocalDate
import org.threeten.bp.ZoneId

data class EditTaskScreen(val taskId: Long) : Screen {
    @SuppressLint("NewApi")
    @OptIn(ExperimentalMaterial3Api::class)
    @Composable
    override fun Content() {
        val viewModel: EditTaskViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val navigator = LocalNavigator.currentOrThrow
        val dialogState = rememberMaterialDialogState()
        var showDeleteDialog by remember { mutableStateOf(false) }

        LaunchedEffect(Unit) {
            viewModel.loadTask(taskId)
        }

        LaunchedEffect(state.isSuccess) {
            if (state.isSuccess) {
                navigator.pop()
            }
        }

        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text("Edit Task") },
                    navigationIcon = {
                        IconButton(onClick = { navigator.pop() }) {
                            Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                        }
                    },
                    actions = {
                        IconButton(onClick = { showDeleteDialog = true }) {
                            Icon(Icons.Default.Delete, contentDescription = "Delete Task")
                        }
                    },
                    colors = TopAppBarDefaults.topAppBarColors(
                        containerColor = MaterialTheme.colorScheme.background,
                        titleContentColor = MaterialTheme.colorScheme.primary
                    )
                )
            }
        ) { padding ->
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
            ) {
                if (state.isLoading) {
                    CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.Center)
                    )
                } else {
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .padding(16.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        TaskTitleField(
                            value = state.title,
                            onValueChange = viewModel::updateTitle,
                            error = state.validation.titleError
                        )

                        TaskDescriptionField(
                            value = state.description,
                            onValueChange = viewModel::updateDescription,
                            error = state.validation.descriptionError
                        )

                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(8.dp)
                        ) {
                            OutlinedButton(
                                onClick = { dialogState.show() },
                                modifier = Modifier.weight(1f),
                                shape = RoundedCornerShape(12.dp)
                            ) {
                                Icon(Icons.Default.CalendarToday, contentDescription = null)
                                Spacer(Modifier.width(8.dp))
                                Text(
                                    when (val deadline = state.deadline) {
                                        null -> "Set Deadline"
                                        else -> deadline.toLocalDateTime(TimeZone.currentSystemDefault()).date.toString()
                                    }
                                )
                            }

                            if (state.deadline != null) {
                                IconButton(
                                    onClick = { viewModel.updateDeadline(null) }
                                ) {
                                    Icon(Icons.Default.Close, contentDescription = "Clear date")
                                }
                            }
                        }

                        state.validation.deadlineError?.let { error ->
                            Text(
                                text = error,
                                color = MaterialTheme.colorScheme.error,
                                style = MaterialTheme.typography.bodySmall
                            )
                        }

                        TaskPrioritySelector(
                            selectedPriority = state.priority,
                            onPrioritySelected = viewModel::updatePriority,
                            modifier = Modifier.fillMaxWidth()
                        )

                        TaskTagSelector(
                            availableTags = state.availableTags,
                            selectedTagIds = state.selectedTagIds,
                            onTagSelectionChanged = viewModel::updateSelectedTags,
                            modifier = Modifier.fillMaxWidth()
                        )

                        // Status Selection
                        OutlinedCard(
                            modifier = Modifier.fillMaxWidth()
                        ) {
                            Column(
                                modifier = Modifier.padding(16.dp),
                                verticalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                Text(
                                    "Status",
                                    style = MaterialTheme.typography.titleMedium
                                )

                                Row(
                                    horizontalArrangement = Arrangement.spacedBy(8.dp)
                                ) {
                                    listOf("ToDo", "InProgress", "Done").forEach { status ->
                                        FilterChip(
                                            selected = state.status == status,
                                            onClick = { viewModel.updateStatus(status) },
                                            label = { Text(status) },
                                            leadingIcon = when (status) {
                                                "ToDo" -> Icons.Default.CheckBoxOutlineBlank
                                                "InProgress" -> Icons.Default.Timer
                                                "Done" -> Icons.Default.CheckBox
                                                else -> null
                                            }?.let { icon ->
                                                {
                                                    Icon(
                                                        icon,
                                                        contentDescription = null,
                                                        modifier = Modifier.size(16.dp)
                                                    )
                                                }
                                            }
                                        )
                                    }
                                }
                            }
                        }

                        Spacer(modifier = Modifier.weight(1f))

                        Button(
                            onClick = { viewModel.saveTask() },
                            modifier = Modifier.fillMaxWidth(),
                            enabled = !state.isLoading,
                            colors = ButtonDefaults.buttonColors(
                                containerColor = MaterialTheme.colorScheme.primary,
                                disabledContainerColor = MaterialTheme.colorScheme.tertiary
                            ),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Text("Save Changes")
                        }
                    }
                }

                // Error Snackbar
                state.error?.let { error ->
                    Snackbar(
                        modifier = Modifier
                            .padding(16.dp)
                            .align(Alignment.BottomCenter)
                    ) {
                        Text(error)
                    }
                }

                // Date Picker Dialog
                MaterialDialog(
                    dialogState = dialogState,
                    buttons = {
                        positiveButton("OK")
                        negativeButton("Cancel")
                    }
                ) {
                    datepicker { date ->
                        val instant = LocalDate.of(date.year, date.monthValue, date.dayOfMonth)
                            .atStartOfDay(ZoneId.systemDefault())
                            .toInstant()
                        viewModel.updateDeadline(instant as Instant?)
                    }
                }

                // Delete Confirmation Dialog
                if (showDeleteDialog) {
                    AlertDialog(
                        onDismissRequest = { showDeleteDialog = false },
                        title = { Text("Delete Task") },
                        text = { Text("Are you sure you want to delete this task? This action cannot be undone.") },
                        confirmButton = {
                            TextButton(
                                onClick = {
                                    viewModel.deleteTask()
                                    showDeleteDialog = false
                                },
                                colors = ButtonDefaults.textButtonColors(
                                    contentColor = MaterialTheme.colorScheme.error
                                )
                            ) {
                                Text("Delete")
                            }
                        },
                        dismissButton = {
                            TextButton(onClick = { showDeleteDialog = false }) {
                                Text("Cancel")
                            }
                        }
                    )
                }
            }
        }
    }
}