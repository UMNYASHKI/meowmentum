package org.meowmentum.project.ui.screens.task.edit

import androidx.compose.runtime.*
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.compose.koinInject
import org.koin.core.component.inject
import org.meowmentum.project.ui.components.TaskForm

data class EditTaskScreen(val taskId: Long) : Screen {
    @Composable
    override fun Content() {
        val viewModel: EditTaskViewModel = koinInject()
        val navigator = LocalNavigator.currentOrThrow
        val task by viewModel.task.collectAsState()
        val isLoading by viewModel.isLoading.collectAsState()

        LaunchedEffect(taskId) {
            viewModel.loadTask(taskId)
        }

        TaskForm(
            title = "Edit Task",
            initialTask = task,
            isLoading = isLoading,
            onSave = { title, description, dueDate, priority, tag ->
                viewModel.updateTask(taskId, title, description, dueDate, priority, tag)
                navigator.pop()
            },
            onBack = { navigator.pop() }
        )
    }
}
