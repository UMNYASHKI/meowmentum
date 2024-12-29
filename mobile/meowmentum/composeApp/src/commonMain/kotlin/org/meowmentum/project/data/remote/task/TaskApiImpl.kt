package org.meowmentum.project.data.remote.task

import io.ktor.client.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.*

class TaskApiImpl(
    private val client: HttpClient,
    private val baseUrl: String = "http://10.0.2.2:8080/api/core/api/tasks"
) : TaskApi {
    override suspend fun upsertTask(id: Long?, request: TaskRequest): ApiResult<TaskResponse> =
        handleApiResponse {
            client.post("$baseUrl${id?.let { "?id=$it" } ?: ""}") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun deleteTask(id: Long): ApiResult<Unit> =
        handleApiResponse {
            client.delete("$baseUrl/$id")
        }

    override suspend fun getTasks(filter: TaskFilterRequest): ApiResult<List<TaskResponse>> =
        handleApiResponse {
            client.get(baseUrl) {
                url {
                    // Only add parameters if they have values
                    filter.taskId?.let { parameters.append("TaskId", it.toString()) }

                    filter.status?.let { statusList ->
                        if (statusList.isNotEmpty()) {
                            parameters.append("Status", statusList.joinToString(","))
                        }
                    }

                    filter.tagIds?.let { tagIds ->
                        if (tagIds.isNotEmpty()) {
                            parameters.append("TagIds", tagIds.joinToString(","))
                        }
                    }

                    filter.priorities?.let { priorities ->
                        if (priorities.isNotEmpty()) {
                            parameters.append("Priorities", priorities.joinToString(","))
                        }
                    }
                }
            }
        }
}