package org.meowmentum.project.data.remote.timer

import io.ktor.client.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.*

class TimerApiImpl(
    private val client: HttpClient,
    private val baseUrl: String = "http://10.0.2.2:8080/api/core/api/timer"
) : TimerApi {
    override suspend fun startTimer(taskId: Long): ApiResult<TimeIntervalResponse> =
        handleApiResponse {
            client.post("$baseUrl/start") {
                parameter("taskId", taskId)
            }
        }

    override suspend fun stopTimer(taskId: Long): ApiResult<TimeIntervalResponse> =
        handleApiResponse {
            client.post("$baseUrl/stop") {
                parameter("taskId", taskId)
            }
        }

    override suspend fun manualLogTime(request: ManualLogRequest): ApiResult<TimeIntervalResponse> =
        handleApiResponse {
            client.post("$baseUrl/log") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun updateTimer(id: Long, request: TimerUpdateRequest): ApiResult<TimeIntervalResponse> =
        handleApiResponse {
            client.put("$baseUrl/$id") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun deleteTimer(id: Long): ApiResult<TimeIntervalResponse> =
        handleApiResponse {
            client.delete("$baseUrl/$id")
        }

    override suspend fun getTimers(taskId: Long?, timeIntervalId: Long?): ApiResult<List<TimeIntervalResponse>> =
        handleApiResponse {
            client.get(baseUrl) {
                parameter("taskId", taskId)
                parameter("timeIntervalId", timeIntervalId)
            }
        }
}