package org.meowmentum.project.domain.model

import kotlinx.datetime.Instant
import org.meowmentum.project.data.models.TaskPriority
import org.meowmentum.project.data.models.TaskStatus

data class Task(
    val id: Long = 0,
    val title: String,
    val description: String? = null,
    val createdAt: Instant? = null,
    val deadline: Instant? = null,
    val completedAt: Instant? = null,
    val status: TaskStatus = TaskStatus.PENDING,
    val priority: TaskPriority? = null,
    val tags: List<Tag>? = null,
    val timeIntervals: List<TimeInterval>? = null
)