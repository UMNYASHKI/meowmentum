package org.meowmentum.project.ui.screens.task.edit

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
import org.meowmentum.project.ui.screens.task.create.ValidationState

class EditTaskViewModel(
    private val taskRepository: TaskRepository,
    private val tagRepository: TagRepository
) : ViewModel() {
    private val _state = MutableStateFlow(EditTaskState())
    val state = _state.asStateFlow()

    init {
        viewModelScope.launch {
            loadTags()
        }
    }

    fun loadTask(taskId: Long) {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            // Create filter request with all necessary parameters
            taskRepository.getTasks(
                taskId = taskId,
                status = null,  // Don't filter by status for single task
                tagIds = null,  // Don't filter by tags for single task
                priorities = null  // Don't filter by priorities for single task
            )
                .onSuccess { tasks ->
                    tasks.firstOrNull()?.let { task ->
                        _state.update {
                            it.copy(
                                taskId = task.id,
                                title = task.title,
                                description = task.description ?: "",
                                deadline = task.deadline,
                                priority = task.priority,
                                status = task.status,
                                selectedTagIds = task.tags?.map { tag -> tag.id } ?: emptyList(),
                                error = null
                            )
                        }
                    } ?: _state.update {
                        it.copy(error = "Task not found")
                    }
                }
                .onFailure { error ->
                    _state.update { it.copy(error = error.message) }
                }

            _state.update { it.copy(isLoading = false) }
        }
    }

    private suspend fun loadTags() {
        tagRepository.getAllTags()
            .onSuccess { tags -> _state.update { it.copy(availableTags = tags) } }
            .onFailure { error -> _state.update { it.copy(error = error.message) } }
    }

    fun updateTitle(title: String) {
        _state.update {
            it.copy(
                title = title,
                validation = it.validation.copy(titleError = validateTitle(title))
            )
        }
    }

    fun updateDescription(description: String) {
        _state.update { it.copy(description = description) }
    }

    fun updateDeadline(deadline: Instant?) {
        _state.update {
            it.copy(
                deadline = deadline,
                validation = it.validation.copy(deadlineError = validateDeadline(deadline))
            )
        }
    }

    fun updatePriority(priority: TaskPriority?) {
        _state.update { it.copy(priority = priority) }
    }

    fun updateStatus(status: TaskStatus) {
        _state.update { it.copy(status = status) }
    }

    fun updateSelectedTags(tagIds: List<Long>) {
        _state.update { it.copy(selectedTagIds = tagIds) }
    }

    fun saveTask() {
        if (!validateForm()) return
        val taskId = _state.value.taskId ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            val task = Task(
                id = taskId,
                title = _state.value.title.trim(),
                description = _state.value.description.trim().takeIf { it.isNotBlank() },
                deadline = _state.value.deadline,
                priority = _state.value.priority,
                status = _state.value.status,
                tags = _state.value.selectedTagIds.let { ids ->
                    _state.value.availableTags.filter { it.id in ids }
                }
            )

            taskRepository.upsertTask(task)
                .onSuccess {
                    _state.update { it.copy(isSuccess = true, error = null) }
                }
                .onFailure { error ->
                    _state.update { it.copy(error = error.message) }
                }

            _state.update { it.copy(isLoading = false) }
        }
    }

    fun deleteTask() {
        val taskId = _state.value.taskId ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            taskRepository.deleteTask(taskId)
                .onSuccess {
                    _state.update { it.copy(isSuccess = true, error = null) }
                }
                .onFailure { error ->
                    _state.update { it.copy(error = error.message) }
                }

            _state.update { it.copy(isLoading = false) }
        }
    }

    private fun validateForm(): Boolean {
        val titleError = validateTitle(_state.value.title)
        val deadlineError = validateDeadline(_state.value.deadline)

        _state.update {
            it.copy(
                validation = ValidationState(
                    titleError = titleError,
                    deadlineError = deadlineError
                )
            )
        }

        return titleError == null && deadlineError == null
    }

    private fun validateTitle(title: String): String? {
        return when {
            title.isBlank() -> "Title cannot be empty"
            title.length < 3 -> "Title must be at least 3 characters long"
            title.length > 100 -> "Title must not exceed 100 characters"
            else -> null
        }
    }

    private fun validateDeadline(deadline: Instant?): String? {
        if (deadline == null) return null

        return when {
            deadline < Clock.System.now() -> "Deadline cannot be in the past"
            else -> null
        }
    }
}

data class EditTaskState(
    val taskId: Long? = null,
    val title: String = "",
    val description: String = "",
    val deadline: Instant? = null,
    val priority: TaskPriority? = null,
    val status: TaskStatus = TaskStatus.PENDING,
    val selectedTagIds: List<Long> = emptyList(),
    val availableTags: List<Tag> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
    val isSuccess: Boolean = false,
    val validation: ValidationState = ValidationState()
)