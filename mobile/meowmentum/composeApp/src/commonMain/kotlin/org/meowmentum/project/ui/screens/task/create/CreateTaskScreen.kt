package org.meowmentum.project.ui.screens.task.create

import androidx.compose.runtime.*
import cafe.adriel.voyager.core.screen.Screen
import cafe.adriel.voyager.navigator.LocalNavigator
import cafe.adriel.voyager.navigator.currentOrThrow
import org.koin.core.component.inject
import org.meowmentum.project.ui.components.TaskForm

class CreateTaskScreen : Screen {
    @Composable
    override fun Content() {
        val viewModel: CreateTaskViewModel by inject()
        val navigator = LocalNavigator.currentOrThrow
        val isLoading by viewModel.isLoading.collectAsState()

        TaskForm(
            title = "Create Task",
            isLoading = isLoading,
            onSave = { title, description, dueDate, priority, tag ->
                viewModel.createTask(title, description, dueDate, priority, tag)
                navigator.pop()
            },
            onBack = { navigator.pop() }
        )
    }
}

