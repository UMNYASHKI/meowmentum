package org.meowmentum.project.domain.repository

import kotlinx.coroutines.flow.Flow
import org.meowmentum.project.domain.model.TimeInterval

interface TimerRepository {
    suspend fun startTimer(taskId: Long): Result<TimeInterval>
    suspend fun stopTimer(taskId: Long): Result<TimeInterval>
    suspend fun logManualTime(
        taskId: Long,
        startTime: kotlinx.datetime.Instant,
        endTime: kotlinx.datetime.Instant,
        description: String?
    ): Result<TimeInterval>
    suspend fun updateTimer(
        id: Long,
        startTime: kotlinx.datetime.Instant?,
        endTime: kotlinx.datetime.Instant?,
        description: String?
    ): Result<TimeInterval>
    suspend fun deleteTimer(id: Long): Result<TimeInterval>
    suspend fun getTimers(taskId: Long? = null, timeIntervalId: Long? = null): Result<List<TimeInterval>>
    fun observeActiveTimer(): Flow<TimeInterval?>
}