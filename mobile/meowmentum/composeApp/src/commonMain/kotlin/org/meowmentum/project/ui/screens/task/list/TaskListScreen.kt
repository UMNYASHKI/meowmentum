package org.meowmentum.project.ui.screens.task.list

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
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
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.ui.components.navigation.AppBottomNavigation
import org.meowmentum.project.ui.components.navigation.NavigationItem
import org.meowmentum.project.ui.components.task.*
import org.meowmentum.project.ui.screens.profile.ProfileScreen
import org.meowmentum.project.ui.screens.task.create.CreateTaskScreen
import org.meowmentum.project.ui.screens.task.edit.EditTaskScreen
import org.meowmentum.project.ui.screens.timer.TimerScreen

class TaskListScreen : Screen {
    @OptIn(ExperimentalMaterial3Api::class)
    @Composable
    override fun Content() {
        val viewModel: TaskListViewModel = koinInject()
        val state by viewModel.state.collectAsState()
        val navigator = LocalNavigator.currentOrThrow

        Scaffold(
            topBar = {
                CenterAlignedTopAppBar(
                    title = { Text("Tasks") },
                    colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                        containerColor = MaterialTheme.colorScheme.background,
                        titleContentColor = MaterialTheme.colorScheme.primary
                    )
                )
            },
            bottomBar = {
                AppBottomNavigation(
                    currentRoute = NavigationItem.TASKS.route,
                    onNavigate = { item ->
                        when (item) {
                            NavigationItem.TIMER -> navigator.push(TimerScreen())
                            NavigationItem.PROFILE -> { navigator.push(ProfileScreen()) }
                            else -> { /* Already on tasks */ }
                        }
                    }
                )
            },
            floatingActionButton = {
                FloatingActionButton(
                    onClick = { navigator.push(CreateTaskScreen()) },
                    containerColor = MaterialTheme.colorScheme.primary,
                    contentColor = MaterialTheme.colorScheme.onPrimary
                ) {
                    Icon(Icons.Default.Add, contentDescription = "Create Task")
                }
            }
        ) { padding ->
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
            ) {
                Column {
                    SearchBar(
                        query = state.filters.searchQuery,
                        onQueryChange = viewModel::updateSearchQuery
                    )

                    if (state.isLoading) {
                        LinearProgressIndicator(
                            modifier = Modifier.fillMaxWidth()
                        )
                    }

                    TaskList(
                        tasks = state.tasks,
                        onTaskClick = { taskId -> navigator.push(EditTaskScreen(taskId)) },
                        onTaskStatusChange = { taskId, isCompleted ->
                            viewModel.updateTaskStatus(
                                taskId = taskId,
                                newStatus = if (isCompleted) "Done" else "ToDo"
                            )
                        }
                    )
                }

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

    @Composable
    private fun TaskList(
        tasks: List<Task>,
        onTaskClick: (Long) -> Unit,
        onTaskStatusChange: (Long, Boolean) -> Unit,
        modifier: Modifier = Modifier
    ) {
        LazyColumn(
            modifier = modifier,
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            // Active Tasks
            item {
                ListHeader(
                    title = "Active Tasks",
                    count = tasks.count { !it.status.equals("Done", ignoreCase = true) }
                )
            }

            items(
                items = tasks.filter { !it.status.equals("Done", ignoreCase = true) },
                key = { it.id }
            ) { task ->
                TaskListItem(
                    title = task.title,
                    description = task.description,
                    isCompleted = false,
                    date = task.deadline,
                    priority = task.priority,
                    tag = task.tags?.firstOrNull(),
                    onStatusChange = { isCompleted ->
                        onTaskStatusChange(task.id, isCompleted)
                    },
                    onItemClick = { onTaskClick(task.id) }
                )
            }

            // Completed Tasks
            item {
                ListHeader(
                    title = "Completed Tasks",
                    count = tasks.count { it.status.equals("Done", ignoreCase = true) }
                )
            }

            items(
                items = tasks.filter { it.status.equals("Done", ignoreCase = true) },
                key = { it.id }
            ) { task ->
                TaskListItem(
                    title = task.title,
                    description = task.description,
                    isCompleted = true,
                    date = task.deadline,
                    priority = task.priority,
                    tag = task.tags?.firstOrNull(),
                    onStatusChange = { isCompleted ->
                        onTaskStatusChange(task.id, isCompleted)
                    },
                    onItemClick = { onTaskClick(task.id) }
                )
            }
        }
    }

    @Composable
    private fun ListHeader(
        title: String,
        count: Int,
        modifier: Modifier = Modifier
    ) {
        Row(
            modifier = modifier
                .fillMaxWidth()
                .padding(vertical = 8.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text(
                text = title,
                style = MaterialTheme.typography.titleMedium,
                color = MaterialTheme.colorScheme.primary
            )
            Text(
                text = count.toString(),
                style = MaterialTheme.typography.labelMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )
        }
    }
}

