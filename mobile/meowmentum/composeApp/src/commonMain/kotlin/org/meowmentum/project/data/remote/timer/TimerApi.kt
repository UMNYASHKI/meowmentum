package org.meowmentum.project.data.remote.timer

import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult

interface TimerApi {
    suspend fun startTimer(taskId: Long): ApiResult<TimeIntervalResponse>
    suspend fun stopTimer(taskId: Long): ApiResult<TimeIntervalResponse>
    suspend fun manualLogTime(request: ManualLogRequest): ApiResult<TimeIntervalResponse>
    suspend fun updateTimer(id: Long, request: TimerUpdateRequest): ApiResult<TimeIntervalResponse>
    suspend fun deleteTimer(id: Long): ApiResult<TimeIntervalResponse>
    suspend fun getTimers(taskId: Long? = null, timeIntervalId: Long? = null): ApiResult<List<TimeIntervalResponse>>
}