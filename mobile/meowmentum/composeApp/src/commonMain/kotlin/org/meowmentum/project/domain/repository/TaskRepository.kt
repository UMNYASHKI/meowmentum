package org.meowmentum.project.domain.repository

import org.meowmentum.project.domain.model.Task

interface TaskRepository {
    suspend fun createTask(task: Task): Result<Task>
    suspend fun updateTask(id: Long, task: Task): Result<Task>
    suspend fun deleteTask(id: Long): Result<Unit>
    suspend fun getTasks(): Result<List<Task>>
}