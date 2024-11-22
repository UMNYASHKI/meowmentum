package org.meowmentum.project.domain.model

import org.meowmentum.project.data.models.UserDto

data class User(
    val id: String,
    val email: String,
    val name: String
)

fun UserDto.toDomain() = User(
    id = id,
    email = email,
    name = name
)
