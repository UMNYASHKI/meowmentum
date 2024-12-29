package org.meowmentum.project.data.remote.task

import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult

interface TaskApi {
    /**
     * Creates or updates a task
     * @param id Optional task ID. If provided, updates existing task; if null, creates new task
     * @param request Task data
     * @return ApiResult containing TaskResponse on success
     */
    suspend fun upsertTask(id: Long?, request: TaskRequest): ApiResult<TaskResponse>

    /**
     * Deletes a task by ID
     * @param id Task ID to delete
     * @return ApiResult containing Unit on success
     */
    suspend fun deleteTask(id: Long): ApiResult<Unit>

    /**
     * Gets tasks based on filter criteria
     * @param filter Filter criteria for tasks
     * @return ApiResult containing list of TaskResponse on success
     */
    suspend fun getTasks(filter: TaskFilterRequest): ApiResult<List<TaskResponse>>
}