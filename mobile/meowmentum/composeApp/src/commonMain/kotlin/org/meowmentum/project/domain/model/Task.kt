package org.meowmentum.project.domain.model

data class Task(
    val id: Long = 0,
    val title: String,
    val description: String? = null,
    val dueDate: String? = null,
    val isCompleted: Boolean = false,
    val userId: String? = null
)