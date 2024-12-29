package org.meowmentum.project.data.repository

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import org.meowmentum.project.data.models.ManualLogRequest
import org.meowmentum.project.data.models.TimerUpdateRequest
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.data.remote.mapper.toDomain
import org.meowmentum.project.data.remote.timer.TimerApi
import org.meowmentum.project.domain.model.TimeInterval
import org.meowmentum.project.domain.repository.TimerRepository

class TimerRepositoryImpl(
    private val api: TimerApi
) : TimerRepository {
    private val _activeTimer = MutableStateFlow<TimeInterval?>(null)

    override suspend fun startTimer(taskId: Long): Result<TimeInterval> {
        return when (val result = api.startTimer(taskId)) {
            is ApiResult.Success -> {
                val timer = result.data.toDomain()
                _activeTimer.value = timer
                Result.success(timer)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun stopTimer(taskId: Long): Result<TimeInterval> {
        return when (val result = api.stopTimer(taskId)) {
            is ApiResult.Success -> {
                _activeTimer.value = null
                Result.success(result.data.toDomain())
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun logManualTime(
        taskId: Long,
        startTime: kotlinx.datetime.Instant,
        endTime: kotlinx.datetime.Instant,
        description: String?
    ): Result<TimeInterval> {
        val request = ManualLogRequest(taskId, startTime, endTime, description)
        return when (val result = api.manualLogTime(request)) {
            is ApiResult.Success -> Result.success(result.data.toDomain())
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun updateTimer(
        id: Long,
        startTime: kotlinx.datetime.Instant?,
        endTime: kotlinx.datetime.Instant?,
        description: String?
    ): Result<TimeInterval> {
        val request = TimerUpdateRequest(startTime, endTime, description)
        return when (val result = api.updateTimer(id, request)) {
            is ApiResult.Success -> Result.success(result.data.toDomain())
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun deleteTimer(id: Long): Result<TimeInterval> {
        return when (val result = api.deleteTimer(id)) {
            is ApiResult.Success -> Result.success(result.data.toDomain())
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun getTimers(
        taskId: Long?,
        timeIntervalId: Long?
    ): Result<List<TimeInterval>> {
        return when (val result = api.getTimers(taskId, timeIntervalId)) {
            is ApiResult.Success -> Result.success(result.data.map { it.toDomain() })
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override fun observeActiveTimer(): Flow<TimeInterval?> = _activeTimer
}
