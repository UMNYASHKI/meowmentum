package org.meowmentum.project.ui.screens.timer

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
import org.koin.compose.koinInject
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.ui.components.navigation.AppBottomNavigation
import org.meowmentum.project.ui.components.navigation.NavigationItem
import org.meowmentum.project.ui.components.timer.*
import org.meowmentum.project.ui.screens.profile.ProfileScreen
import org.meowmentum.project.ui.screens.task.list.TaskListScreen

class TimerScreen(val initialTaskId: Long? = null) : Screen {
    @OptIn(ExperimentalMaterial3Api::class)
    @Composable
    override fun Content() {
        val viewModel: TimerViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text("Timer") },
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
            },
            bottomBar = {
                AppBottomNavigation(
                    currentRoute = NavigationItem.TIMER.route,
                    onNavigate = { item ->
                        when (item) {
                            NavigationItem.TASKS -> navigator.push(TaskListScreen())
                            NavigationItem.PROFILE -> { navigator.push(ProfileScreen()) }
                            else -> { /* Already on timer */ }
                        }
                    }
                )
            }
        ) { padding ->
            // Existing Timer content
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
            ) {
                when (state.selectedTask) {
                    null -> TaskSelectionContent(
                        state = state,
                        onTaskSelect = viewModel::selectTask,
                        onSearchQueryChange = viewModel::updateSearchQuery,
                        onStart = { viewModel.startTimer() }
                    )
                    else -> TimerContent(
                        state = state,
                        onStart = { viewModel.startTimer() },
                        onPause = { viewModel.pauseTimer() },
                        onResume = { viewModel.resumeTimer() },
                        onStop = { viewModel.stopTimer() },
                        onTaskClick = { /* Navigate to task details */ }
                    )
                }

                // Loading indicator
                if (state.isLoading) {
                    CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.Center)
                    )
                }

                // Error snackbar
                state.error?.let { error ->
                    Snackbar(
                        modifier = Modifier
                            .align(Alignment.BottomCenter)
                            .padding(16.dp)
                    ) {
                        Text(error)
                    }
                }
            }
        }
    }
}

@Composable
private fun TaskSelectionContent(
    state: TimerScreenState,
    onTaskSelect: (Task) -> Unit,
    onSearchQueryChange: (String) -> Unit,
    onStart: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier
            .fillMaxSize()
            .padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        TimerDisplay(
            elapsedTimeMillis = 0,
            modifier = Modifier.padding(vertical = 32.dp)
        )

        Button(
            onClick = onStart,
            enabled = true,
            modifier = Modifier
                .fillMaxWidth()
                .padding(bottom = 32.dp),
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary,
                disabledContainerColor = MaterialTheme.colorScheme.tertiary
            ),
            shape = RoundedCornerShape(12.dp)
        ) {
            Text("Start Timer")
        }

        Text(
            text = "Select Task",
            style = MaterialTheme.typography.titleLarge,
            modifier = Modifier.padding(bottom = 16.dp)
        )

        TaskSearchBar(
            query = state.searchQuery,
            onQueryChange = onSearchQueryChange
        )

        TaskList(
            tasks = state.availableTasks,
            onTaskSelect = onTaskSelect,
            modifier = Modifier.weight(1f)
        )
    }
}

@Composable
private fun TimerContent(
    state: TimerScreenState,
    onStart: () -> Unit,
    onPause: () -> Unit,
    onResume: () -> Unit,
    onStop: () -> Unit,
    onTaskClick: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier
            .fillMaxSize()
            .padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        state.selectedTask?.let { task ->
            SelectedTaskDisplay(
                task = task,
                onTaskClick = onTaskClick
            )
        }

        Spacer(modifier = Modifier.height(32.dp))

        TimerDisplay(
            elapsedTimeMillis = state.elapsedTime,
            modifier = Modifier.padding(vertical = 32.dp)
        )

        TimerControls(
            timerState = state.timerState,
            onStart = onStart,
            onPause = onPause,
            onResume = onResume,
            onStop = onStop,
            modifier = Modifier.padding(horizontal = 32.dp)
        )
    }
}