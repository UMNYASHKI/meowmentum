package org.meowmentum.project.data.repository

import org.meowmentum.project.data.models.TaskRequest
import org.meowmentum.project.data.remote.TaskApi
import org.meowmentum.project.domain.model.Task
import org.meowmentum.project.domain.repository.TaskRepository

class TaskRepositoryImpl(
    private val api: TaskApi
) : TaskRepository {
    override suspend fun createTask(task: Task): Result<Task> {
        val request = TaskRequest(
            title = task.title,
            description = task.description,
            dueDate = task.dueDate,
            isCompleted = task.isCompleted
        )
        return api.createTask(request).map { response ->
            Task(
                id = response.id,
                title = response.title,
                description = response.description,
                dueDate = response.dueDate,
                isCompleted = response.isCompleted,
                userId = response.userId
            )
        }
    }

    override suspend fun updateTask(id: Long, task: Task): Result<Task> {
        val request = TaskRequest(
            title = task.title,
            description = task.description,
            dueDate = task.dueDate,
            isCompleted = task.isCompleted
        )
        return api.updateTask(id, request).map { response ->
            Task(
                id = response.id,
                title = response.title,
                description = response.description,
                dueDate = response.dueDate,
                isCompleted = response.isCompleted,
                userId = response.userId
            )
        }
    }

    override suspend fun deleteTask(id: Long): Result<Unit> = api.deleteTask(id)

    override suspend fun getTasks(): Result<List<Task>> {
        return api.getTasks().map { responses ->
            responses.map { response ->
                Task(
                    id = response.id,
                    title = response.title,
                    description = response.description,
                    dueDate = response.dueDate,
                    isCompleted = response.isCompleted,
                    userId = response.userId
                )
            }
        }
    }
}