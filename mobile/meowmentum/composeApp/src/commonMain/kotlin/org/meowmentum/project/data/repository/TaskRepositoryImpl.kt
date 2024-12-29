import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.data.remote.mapper.toDomain
import org.meowmentum.project.data.remote.task.TaskApi
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.repository.TaskRepository

class TaskRepositoryImpl(
    private val api: TaskApi
) : TaskRepository {
    private val _tasks = MutableStateFlow<List<Task>>(emptyList())

    override suspend fun upsertTask(task: Task): Result<Task> {
        val request = TaskRequest(
            title = task.title.trim(),
            description = task.description?.trim(),
            deadline = task.deadline,
            priority = task.priority,
            status = task.status,
            tagIds = task.tags?.map { it.id }?.takeIf { it.isNotEmpty() }
        )

        return when (val result = api.upsertTask(task.id.takeIf { it != 0L }, request)) {
            is ApiResult.Success -> {
                val updatedTask = result.data.toDomain()
                updateLocalTasks(updatedTask)
                Result.success(updatedTask)
            }
            is ApiResult.Error -> {
                println("Error upserting task: ${result.message}")
                Result.failure(Exception(result.message))
            }
        }
    }

    override suspend fun deleteTask(id: Long): Result<Unit> {
        return when (val result = api.deleteTask(id)) {
            is ApiResult.Success -> {
                removeLocalTask(id)
                Result.success(Unit)
            }
            is ApiResult.Error -> {
                println("Error deleting task: ${result.message}")
                Result.failure(Exception(result.message))
            }
        }
    }

    override suspend fun getTasks(
        taskId: Long?,
        status: List<TaskStatus>?,
        tagIds: List<Long>?,
        priorities: List<TaskPriority>?
    ): Result<List<Task>> {
        val filter = TaskFilterRequest(
            taskId = taskId,
            status = status?.takeIf { it.isNotEmpty() },
            tagIds = tagIds?.takeIf { it.isNotEmpty() },
            priorities = priorities?.takeIf { it.isNotEmpty() }
        )

        return when (val result = api.getTasks(filter)) {
            is ApiResult.Success -> {
                val tasks = result.data.map { it.toDomain() }
                if (taskId == null) { // Only update cache for full list requests
                    _tasks.value = tasks
                }
                Result.success(tasks)
            }
            is ApiResult.Error -> {
                println("Error getting tasks: ${result.message}")
                // If the error is "no tasks found", return empty list instead of error
                if (result.message.contains("No tasks found", ignoreCase = true)) {
                    Result.success(emptyList())
                } else {
                    Result.failure(Exception(result.message))
                }
            }
        }
    }

    override fun observeTasks(): Flow<List<Task>> = _tasks

    private fun updateLocalTasks(task: Task) {
        val currentTasks = _tasks.value.toMutableList()
        val index = currentTasks.indexOfFirst { it.id == task.id }
        if (index != -1) {
            currentTasks[index] = task
        } else {
            currentTasks.add(0, task) // Add new tasks at the beginning
        }
        _tasks.value = currentTasks
    }

    private fun removeLocalTask(taskId: Long) {
        _tasks.value = _tasks.value.filter { it.id != taskId }
    }
}