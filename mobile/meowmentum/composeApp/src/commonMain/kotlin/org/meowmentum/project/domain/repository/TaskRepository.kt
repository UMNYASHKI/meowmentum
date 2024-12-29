package org.meowmentum.project.domain.repository

import kotlinx.coroutines.flow.Flow
import org.meowmentum.project.data.models.TaskPriority
import org.meowmentum.project.data.models.TaskStatus
import org.meowmentum.project.domain.model.Task

interface TaskRepository {
    suspend fun upsertTask(task: Task): Result<Task>
    suspend fun deleteTask(id: Long): Result<Unit>
    suspend fun getTasks(
        taskId: Long?,
        status: List<TaskStatus>?,
        tagIds: List<Long>?,
        priorities: List<TaskPriority>?
    ): Result<List<Task>>
    fun observeTasks(): Flow<List<Task>>
}