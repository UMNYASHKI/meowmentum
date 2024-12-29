package org.meowmentum.project.ui.screens.task.create

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

class CreateTaskScreen : Screen {
    @SuppressLint("NewApi")
    @OptIn(ExperimentalMaterial3Api::class)
    @Composable
    override fun Content() {
        var showDatePicker by remember { mutableStateOf(false) }
        val viewModel: CreateTaskViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val navigator = LocalNavigator.currentOrThrow
        val dialogState = rememberMaterialDialogState()

        LaunchedEffect(state.isSuccess) {
            if (state.isSuccess) {
                navigator.pop()
            }
        }

        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text("Create Task") },
                    navigationIcon = {
                        IconButton(onClick = { navigator.pop() }) {
                            Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
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
                            onClick = { showDatePicker = true },
                            modifier = Modifier.weight(1f),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Icon(Icons.Default.CalendarToday, contentDescription = null)
                            Spacer(Modifier.width(8.dp))
                            Text(
                                when (val deadline = state.deadline) {
                                    null -> "Set Deadline"
                                    else -> deadline.toLocalDateTime(TimeZone.currentSystemDefault())
                                        .date.toString()
                                }
                            )
                        }

                        // Clear date button
                        if (state.deadline != null) {
                            IconButton(onClick = { viewModel.updateDeadline(null) }) {
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

                    Spacer(modifier = Modifier.weight(1f))

                    Button(
                        onClick = { viewModel.createTask() },
                        enabled = !state.isLoading,
                        modifier = Modifier.fillMaxWidth(),
                        colors = ButtonDefaults.buttonColors(
                            containerColor = MaterialTheme.colorScheme.primary,
                            disabledContainerColor = MaterialTheme.colorScheme.tertiary
                        ),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        if (state.isLoading) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(24.dp),
                                color = MaterialTheme.colorScheme.onPrimary
                            )
                        } else {
                            Text("Create Task")
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
            }
        }
    }
}

