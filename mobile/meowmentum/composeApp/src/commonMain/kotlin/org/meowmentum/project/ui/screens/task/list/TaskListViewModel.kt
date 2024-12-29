package org.meowmentum.project.ui.screens.task.list

import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import kotlinx.datetime.Clock
import kotlinx.datetime.Instant
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.data.models.TaskPriority
import org.meowmentum.project.data.models.TaskStatus
import org.meowmentum.project.domain.model.*
import org.meowmentum.project.domain.repository.*

class TaskListViewModel(
    private val taskRepository: TaskRepository,
    private val tagRepository: TagRepository
) : ViewModel() {
    private val _state = MutableStateFlow(TaskListState())
    val state = _state.asStateFlow()

    init {
        viewModelScope.launch {
            loadInitialData()
            setupObservers()
        }
    }

    private suspend fun loadInitialData() {
        loadTasks()
        loadTags()
    }

    private suspend fun setupObservers() {
        taskRepository.observeTasks().collect { tasks ->
            _state.update { it.copy(tasks = filterTasks(tasks, it.filters)) }
        }

        tagRepository.observeTags().collect { tags ->
            _state.update { it.copy(tags = tags) }
        }
    }

    private suspend fun loadTasks() {
        _state.update { it.copy(isLoading = true) }
        val filters = _state.value.filters

        try {
            taskRepository.getTasks(
                taskId = null,  // No specific task ID when loading list
                status = filters.statusFilter.takeIf { it.isNotEmpty() },
                tagIds = filters.tagIds.takeIf { it.isNotEmpty() },
                priorities = filters.priorities.takeIf { it.isNotEmpty() }
            ).onSuccess { tasks ->
                _state.update {
                    it.copy(
                        tasks = filterTasks(tasks, filters),
                        error = null
                    )
                }
            }.onFailure { error ->
                _state.update { it.copy(error = error.message) }
            }
        } catch (e: Exception) {
            _state.update { it.copy(error = e.message) }
        } finally {
            _state.update { it.copy(isLoading = false) }
        }
    }

    private suspend fun loadTags() {
        tagRepository.getAllTags()
            .onSuccess { tags -> _state.update { it.copy(tags = tags) } }
            .onFailure { error -> _state.update { it.copy(error = error.message) } }
    }

    fun updateSearchQuery(query: String) {
        viewModelScope.launch {
            val newFilters = _state.value.filters.copy(searchQuery = query)
            _state.update { it.copy(filters = newFilters) }
            loadTasks()
        }
    }

    fun updateTaskStatus(taskId: Long, newStatus: TaskStatus) {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            // First get the specific task
            taskRepository.getTasks(
                taskId = taskId,
                status = null,
                tagIds = null,
                priorities = null
            ).onSuccess { tasks ->
                val task = tasks.firstOrNull() ?: run {
                    _state.update {
                        it.copy(
                            error = "Task not found",
                            isLoading = false
                        )
                    }
                    return@launch
                }

                val updatedTask = task.copy(
                    status = newStatus,
                    completedAt = if (newStatus == TaskStatus.COMPLETED)
                        Clock.System.now()
                    else
                        null
                )

                taskRepository.upsertTask(updatedTask)
                    .onSuccess {
                        // Successful update will trigger observer
                        _state.update { it.copy(error = null) }
                    }
                    .onFailure { error ->
                        _state.update { it.copy(error = error.message) }
                    }
            }.onFailure { error ->
                _state.update { it.copy(error = error.message) }
            }

            _state.update { it.copy(isLoading = false) }
        }
    }

    fun updateFilters(
        statusFilter: List<TaskStatus>? = null,
        tagIds: List<Long>? = null,
        priorities: List<TaskPriority>? = null
    ) {
        viewModelScope.launch {
            val currentFilters = _state.value.filters
            val newFilters = currentFilters.copy(
                statusFilter = statusFilter ?: currentFilters.statusFilter,
                tagIds = tagIds ?: currentFilters.tagIds,
                priorities = priorities ?: currentFilters.priorities
            )
            _state.update { it.copy(filters = newFilters) }
            loadTasks()
        }
    }

    fun clearFilters() {
        viewModelScope.launch {
            _state.update { it.copy(filters = TaskListFilters()) }
            loadTasks()
        }
    }

    private fun filterTasks(tasks: List<Task>, filters: TaskListFilters): List<Task> {
        return tasks.filter { task ->
            val matchesSearch = filters.searchQuery.isEmpty() ||
                    task.title.contains(filters.searchQuery, ignoreCase = true) ||
                    (task.description?.contains(filters.searchQuery, ignoreCase = true) == true)

            val matchesStatus = filters.statusFilter.isEmpty() ||
                    task.status in filters.statusFilter

            val matchesTags = filters.tagIds.isEmpty() ||
                    task.tags?.any { it.id in filters.tagIds } == true

            val matchesPriority = filters.priorities.isEmpty() ||
                    task.priority in filters.priorities

            matchesSearch && matchesStatus && matchesTags && matchesPriority
        }.sortedWith(compareBy<Task> {task ->
            when (task.status) {
                TaskStatus.COMPLETED -> 1
                TaskStatus.PENDING -> 0
                TaskStatus.IN_PROGRESS -> -1
            }
        }.thenByDescending { task ->
            task.createdAt ?: Instant.DISTANT_PAST
        })
    }
}

data class TaskListFilters(
    val searchQuery: String = "",
    val statusFilter: List<TaskStatus> = emptyList(),
    val tagIds: List<Long> = emptyList(),
    val priorities: List<TaskPriority> = emptyList()
)

data class TaskListState(
    val tasks: List<Task> = emptyList(),
    val tags: List<Tag> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
    val filters: TaskListFilters = TaskListFilters()
)