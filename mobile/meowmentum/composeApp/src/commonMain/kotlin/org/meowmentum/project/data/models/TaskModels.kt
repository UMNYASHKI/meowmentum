package org.meowmentum.project.data.models

import kotlinx.serialization.Serializable

@Serializable
data class TaskRequest(
    val title: String,
    val description: String?,
    val dueDate: String?,
    val isCompleted: Boolean = false
)

@Serializable
data class TaskResponse(
    val id: Long,
    val title: String,
    val description: String?,
    val dueDate: String?,
    val isCompleted: Boolean,
    val userId: String
)