package org.meowmentum.project.domain.model

import kotlinx.datetime.Instant

data class TimeInterval(
    val id: Long = 0,
    val startTime: Instant,
    val endTime: Instant? = null,
    val description: String? = null,
    val taskId: Long
)