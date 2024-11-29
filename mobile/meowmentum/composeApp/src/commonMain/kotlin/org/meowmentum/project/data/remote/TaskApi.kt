package org.meowmentum.project.data.remote

import org.meowmentum.project.data.models.TaskRequest
import org.meowmentum.project.data.models.TaskResponse

interface TaskApi {
    suspend fun createTask(request: TaskRequest): Result<TaskResponse>
    suspend fun updateTask(id: Long, request: TaskRequest): Result<TaskResponse>
    suspend fun deleteTask(id: Long): Result<Unit>
    suspend fun getTasks(): Result<List<TaskResponse>>
}