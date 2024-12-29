package org.meowmentum.project.data.models

import kotlinx.datetime.Instant
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
enum class TaskStatus {
    @SerialName("Pending")
    PENDING,
    @SerialName("InProgress")
    IN_PROGRESS,
    @SerialName("Completed")
    COMPLETED;

    companion object {
        fun fromString(value: String?): TaskStatus = when(value) {
            "Pending" -> PENDING
            "InProgress" -> IN_PROGRESS
            "Completed" -> COMPLETED
            else -> PENDING
        }
    }
}

@Serializable
enum class TaskPriority {
    @SerialName("High")
    HIGH,
    @SerialName("Medium")
    MEDIUM,
    @SerialName("Low")
    LOW;

    companion object {
        fun fromString(value: String?): TaskPriority = when(value) {
            "High" -> HIGH
            "Medium" -> MEDIUM
            "Low" -> LOW
            else -> MEDIUM
        }
    }
}

@Serializable
data class TaskRequest(
    val title: String,
    val description: String? = null,
    val deadline: Instant? = null,
    val priority: TaskPriority? = null,
    val status: TaskStatus? = null,
    val tagIds: List<Long>? = null
)

@Serializable
data class TaskResponse(
    val id: Long,
    val title: String,
    val description: String?,
    val createdAt: Instant,
    val deadline: Instant?,
    val completedAt: Instant?,
    val status: TaskStatus?,
    val priority: TaskPriority?,
    val tags: List<TagResponse>?,
    val timeIntervals: List<TimeIntervalResponse>?
)

@Serializable
data class TaskFilterRequest(
    val taskId: Long? = null,
    val status: List<TaskStatus>? = null,
    val tagIds: List<Long>? = null,
    val priorities: List<TaskPriority>? = null
)

@Serializable
data class TagRequest(
    val name: String
)

@Serializable
data class TagResponse(
    val id: Long,
    val name: String,
    val createdDate: Instant,
    val updatedDate: Instant?
)

@Serializable
data class TimeIntervalResponse(
    val id: Long,
    val startTime: Instant,
    val endTime: Instant?,
    val description: String?,
    val taskId: Long
)

@Serializable
data class ManualLogRequest(
    val taskId: Long,
    val startTime: Instant,
    val endTime: Instant,
    val description: String?
)

@Serializable
data class TimerUpdateRequest(
    val startTime: Instant?,
    val endTime: Instant?,
    val description: String?
)