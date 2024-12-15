package org.meowmentum.project.ui.screens.task.list

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.repository.TaskRepository

class TaskListViewModel(
    private val taskRepository: TaskRepository
) {
    private val scope = CoroutineScope(Dispatchers.Main)

    private val _tasks = MutableStateFlow<List<Task>>(emptyList())
    val tasks = _tasks.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    private val _error = MutableStateFlow<String?>(null)
    val error = _error.asStateFlow()

    init {
        loadTasks()
    }

    fun loadTasks() {
        scope.launch {
            _isLoading.value = true
            taskRepository.getTasks()
                .onSuccess { _tasks.value = it }
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }

    fun toggleTaskCompletion(task: Task) {
        scope.launch {
            _isLoading.value = true
            val updatedTask = task.copy(isCompleted = !task.isCompleted)
            taskRepository.updateTask(task.id, updatedTask)
                .onSuccess { loadTasks() }
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }
}