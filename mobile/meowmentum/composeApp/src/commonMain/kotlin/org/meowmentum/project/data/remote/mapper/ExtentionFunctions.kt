package org.meowmentum.project.data.remote.mapper

import org.meowmentum.project.data.models.*
import org.meowmentum.project.domain.model.*

// Extension functions for domain model conversion
fun TaskResponse.toDomain() = Task(
    id = id,
    title = title,
    description = description,
    createdAt = createdAt,
    deadline = deadline,
    completedAt = completedAt,
    status = status ?: TaskStatus.PENDING,
    priority = priority,
    tags = tags?.map { it.toDomain() },
    timeIntervals = timeIntervals?.map { it.toDomain() }
)

fun TagResponse.toDomain() = Tag(
    id = id,
    name = name,
    createdDate = createdDate,
    updatedDate = updatedDate
)

fun TimeIntervalResponse.toDomain() = TimeInterval(
    id = id,
    startTime = startTime,
    endTime = endTime,
    description = description,
    taskId = taskId
)