package org.meowmentum.project.ui.screens.timer

import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import kotlinx.coroutines.currentCoroutineContext
import kotlinx.datetime.Clock
import kotlinx.datetime.Instant
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.data.models.TaskStatus
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.model.TimeInterval
import org.meowmentum.project.domain.repository.TaskRepository
import org.meowmentum.project.domain.repository.TimerRepository
import kotlin.time.Duration.Companion.seconds

class TimerViewModel(
    private val timerRepository: TimerRepository,
    private val taskRepository: TaskRepository
) : ViewModel() {
    private val _state = MutableStateFlow(TimerScreenState())
    val state = _state.asStateFlow()

    private var timerJob: Job? = null
    private var activeTimerId: Long? = null

    init {
        viewModelScope.launch {
            loadTasks()
            observeActiveTimer()
        }
    }

    private suspend fun loadTasks() {
        _state.update { it.copy(isLoading = true) }

        taskRepository.getTasks(
            status = listOf(TaskStatus.PENDING, TaskStatus.IN_PROGRESS),  // Only get non-completed tasks
            tagIds = null,
            priorities = null,
            taskId = null
        ).onSuccess { tasks ->
            _state.update { it.copy(
                availableTasks = tasks,
                isLoading = false
            ) }
        }.onFailure { error ->
            _state.update { it.copy(
                error = error.message,
                isLoading = false
            ) }
        }
    }

    fun updateSearchQuery(query: String) {
        viewModelScope.launch {
            _state.update { it.copy(
                searchQuery = query,
                isLoading = true
            ) }

            taskRepository.getTasks(
                status = listOf(TaskStatus.PENDING, TaskStatus.IN_PROGRESS),
                tagIds = null,
                priorities = null,
                taskId = null
            ).onSuccess { allTasks ->
                val filteredTasks = if (query.isBlank()) {
                    allTasks
                } else {
                    allTasks.filter { task ->
                        task.title.contains(query, ignoreCase = true)
                    }
                }
                _state.update { it.copy(
                    availableTasks = filteredTasks,
                    isLoading = false
                ) }
            }.onFailure { error ->
                _state.update { it.copy(
                    error = error.message,
                    isLoading = false
                ) }
            }
        }
    }

    fun selectTask(task: Task) {
        _state.update { it.copy(selectedTask = task) }
    }

    fun startTimer() {
        val selectedTask = state.value.selectedTask ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            // Update task status to IN_PROGRESS
            val updatedTask = selectedTask.copy(status = TaskStatus.IN_PROGRESS)
            taskRepository.upsertTask(updatedTask)
                .onFailure { error ->
                    _state.update { it.copy(
                        error = error.message,
                        isLoading = false
                    ) }
                    return@launch
                }

            // Start the timer
            timerRepository.startTimer(selectedTask.id)
                .onSuccess { timer ->
                    activeTimerId = timer.id
                    _state.update { it.copy(
                        timerState = TimerState.Running,
                        selectedTask = updatedTask,
                        isLoading = false
                    ) }
                    startTimerUpdates(timer.startTime)
                }
                .onFailure { error ->
                    _state.update { it.copy(
                        error = error.message,
                        isLoading = false
                    ) }
                }
        }
    }

    fun stopTimer() {
        val selectedTask = state.value.selectedTask ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            // Update task status
            val updatedTask = selectedTask.copy(status = TaskStatus.COMPLETED)
            taskRepository.upsertTask(updatedTask)
                .onFailure { error ->
                    _state.update { it.copy(
                        error = error.message,
                        isLoading = false
                    ) }
                    return@launch
                }

            // Stop the timer
            timerRepository.stopTimer(selectedTask.id)
                .onSuccess {
                    timerJob?.cancel()
                    _state.update { it.copy(
                        timerState = TimerState.Initial,
                        elapsedTime = 0,
                        selectedTask = null,
                        isLoading = false
                    ) }
                    activeTimerId = null
                }
                .onFailure { error ->
                    _state.update { it.copy(
                        error = error.message,
                        isLoading = false
                    ) }
                }

            // Refresh task list
            loadTasks()
        }
    }

    fun pauseTimer() {
        val selectedTask = state.value.selectedTask ?: return
        val currentTimerId = activeTimerId ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            timerRepository.updateTimer(
                id = currentTimerId,
                startTime = null,
                endTime = Clock.System.now(),
                description = null
            ).onSuccess { timer ->
                timerJob?.cancel()
                _state.update { it.copy(
                    timerState = TimerState.Paused,
                    isLoading = false
                ) }
            }.onFailure { error ->
                _state.update { it.copy(
                    error = error.message,
                    isLoading = false
                ) }
            }
        }
    }

    fun resumeTimer() {
        val selectedTask = state.value.selectedTask ?: return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            timerRepository.startTimer(selectedTask.id)
                .onSuccess { timer ->
                    activeTimerId = timer.id
                    startTimerUpdates(timer.startTime)
                    _state.update { it.copy(
                        timerState = TimerState.Running,
                        isLoading = false
                    ) }
                }
                .onFailure { error ->
                    _state.update { it.copy(
                        error = error.message,
                        isLoading = false
                    ) }
                }
        }
    }

    private fun startTimerUpdates(startTime: Instant) {
        timerJob?.cancel()
        timerJob = viewModelScope.launch {
            while (currentCoroutineContext().isActive) {
                val currentTime = Clock.System.now()
                val elapsedMillis = currentTime.toEpochMilliseconds() - startTime.toEpochMilliseconds()
                _state.update { it.copy(elapsedTime = elapsedMillis) }
                delay(1000) // Update every second
            }
        }
    }

    private suspend fun observeActiveTimer() {
        timerRepository.observeActiveTimer().collectLatest { activeTimer ->
            activeTimer?.let { timer ->
                activeTimerId = timer.id
                val task = state.value.availableTasks.find { it.id == timer.taskId }
                val elapsedMillis = if (timer.endTime != null) {
                    timer.endTime.toEpochMilliseconds() - timer.startTime.toEpochMilliseconds()
                } else {
                    Clock.System.now().toEpochMilliseconds() - timer.startTime.toEpochMilliseconds()
                }

                _state.update {
                    it.copy(
                        selectedTask = task,
                        timerState = if (timer.endTime == null) TimerState.Running else TimerState.Paused,
                        elapsedTime = elapsedMillis
                    )
                }

                if (timer.endTime == null) {
                    startTimerUpdates(timer.startTime)
                }
            }
        }
    }

    override fun onCleared() {
        super.onCleared()
        timerJob?.cancel()
    }
}

enum class TimerState {
    Initial,
    Running,
    Paused
}

data class TimerScreenState(
    val selectedTask: Task? = null,
    val availableTasks: List<Task> = emptyList(),
    val timerState: TimerState = TimerState.Initial,
    val elapsedTime: Long = 0,
    val searchQuery: String = "",
    val isLoading: Boolean = false,
    val error: String? = null
)