package org.meowmentum.project.data.remote

import io.ktor.client.*
import io.ktor.client.call.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.TaskRequest
import org.meowmentum.project.data.models.TaskResponse

class TaskApiImpl(
    private val client: HttpClient
) : TaskApi {
    private val baseUrl = "http://10.0.2.2:8080/api"

    override suspend fun createTask(request: TaskRequest): Result<TaskResponse> = runCatching {
        client.post(baseUrl) {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun updateTask(id: Long, request: TaskRequest): Result<TaskResponse> = runCatching {
        client.put("$baseUrl/$id") {
            contentType(ContentType.Application.Json)
            setBody(request)
        }.body()
    }

    override suspend fun deleteTask(id: Long): Result<Unit> = runCatching {
        client.delete("$baseUrl/$id").body()
    }

    override suspend fun getTasks(): Result<List<TaskResponse>> = runCatching {
        client.get(baseUrl).body()
    }
}