package org.meowmentum.project.domain.model

import kotlinx.datetime.Instant

data class Tag(
    val id: Long = 0,
    val name: String,
    val createdDate: Instant,
    val updatedDate: Instant? = null
)