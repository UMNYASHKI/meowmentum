package org.meowmentum.project.ui.screens.task.create

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

class CreateTaskViewModel(
    private val taskRepository: TaskRepository,
    private val tagRepository: TagRepository
) : ViewModel() {
    private val _state = MutableStateFlow(CreateTaskState())
    val state = _state.asStateFlow()

    init {
        viewModelScope.launch {
            loadTags()
        }
    }

    private suspend fun loadTags() {
        _state.update { it.copy(isLoading = true) }
        tagRepository.getAllTags()
            .onSuccess { tags -> _state.update { it.copy(availableTags = tags) } }
            .onFailure { error -> _state.update { it.copy(error = error.message) } }
        _state.update { it.copy(isLoading = false) }
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

    fun updateSelectedTags(tagIds: List<Long>) {
        _state.update { it.copy(selectedTagIds = tagIds) }
    }

    fun createTask() {
        if (!validateForm()) return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            val task = Task(
                title = _state.value.title.trim(),
                description = _state.value.description.trim().takeIf { it.isNotBlank() },
                deadline = _state.value.deadline,
                priority = _state.value.priority,
                status = TaskStatus.PENDING,
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

data class CreateTaskState(
    val title: String = "",
    val description: String = "",
    val deadline: Instant? = null,
    val priority: TaskPriority? = null,
    val selectedTagIds: List<Long> = emptyList(),
    val availableTags: List<Tag> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
    val isSuccess: Boolean = false,
    val validation: ValidationState = ValidationState()
)

data class ValidationState(
    val titleError: String? = null,
    val deadlineError: String? = null
)