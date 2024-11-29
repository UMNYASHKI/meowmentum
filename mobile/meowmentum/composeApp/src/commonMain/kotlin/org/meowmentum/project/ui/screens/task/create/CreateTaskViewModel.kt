package org.meowmentum.project.ui.screens.task.create

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.repository.TaskRepository

class CreateTaskViewModel(
    private val taskRepository: TaskRepository
) {
    private val scope = CoroutineScope(Dispatchers.Main)

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    private val _error = MutableStateFlow<String?>(null)
    val error = _error.asStateFlow()

    fun createTask(
        title: String,
        description: String?,
        dueDate: String?,
        priority: Int,
        tag: String?
    ) {
        scope.launch {
            _isLoading.value = true
            val task = Task(
                title = title,
                description = description,
                dueDate = dueDate,
                priority = priority,
                tag = tag
            )
            taskRepository.createTask(task)
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }
}