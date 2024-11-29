package org.meowmentum.project.domain.model

data class Task(
    val id: Long = 0,
    val title: String,
    val description: String? = null,
    val dueDate: String? = null,
    val isCompleted: Boolean = false,
    val priority: Int = 0,
    val tag: String? = null,
    val userId: String? = null
)