package org.meowmentum.project.domain.repository

import kotlinx.coroutines.flow.Flow
import org.meowmentum.project.domain.model.Tag

interface TagRepository {
    suspend fun getAllTags(): Result<List<Tag>>
    suspend fun getTagById(tagId: Long): Result<Tag>
    suspend fun createTag(name: String): Result<Unit>
    suspend fun updateTag(tagId: Long, name: String): Result<Unit>
    suspend fun deleteTag(tagId: Long): Result<Unit>
    fun observeTags(): Flow<List<Tag>>
}