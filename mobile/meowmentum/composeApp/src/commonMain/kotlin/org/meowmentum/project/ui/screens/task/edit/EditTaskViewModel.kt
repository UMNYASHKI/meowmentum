package org.meowmentum.project.ui.screens.task.edit

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.repository.TaskRepository

class EditTaskViewModel(
    private val taskRepository: TaskRepository
) {
    private val scope = CoroutineScope(Dispatchers.Main)

    private val _task = MutableStateFlow<Task?>(null)
    val task = _task.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    private val _error = MutableStateFlow<String?>(null)
    val error = _error.asStateFlow()

    fun loadTask(taskId: Long) {
        scope.launch {
            _isLoading.value = true
            taskRepository.getTasks()
                .onSuccess { tasks ->
                    _task.value = tasks.find { it.id == taskId }
                }
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }

    fun updateTask(
        taskId: Long,
        title: String,
        description: String?,
        dueDate: String?,
        priority: Int,
        tag: String?
    ) {
        scope.launch {
            _isLoading.value = true
            val updatedTask = Task(
                id = taskId,
                title = title,
                description = description,
                dueDate = dueDate,
                priority = priority,
                tag = tag,
                isCompleted = _task.value?.isCompleted ?: false
            )
            taskRepository.updateTask(taskId, updatedTask)
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }

    fun deleteTask(taskId: Long) {
        scope.launch {
            _isLoading.value = true
            taskRepository.deleteTask(taskId)
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }
}