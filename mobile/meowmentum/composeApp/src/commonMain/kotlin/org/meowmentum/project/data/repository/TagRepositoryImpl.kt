package org.meowmentum.project.data.repository

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import org.meowmentum.project.data.models.TagRequest
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.data.remote.mapper.toDomain
import org.meowmentum.project.data.remote.tag.TagApi
import org.meowmentum.project.domain.model.Tag
import org.meowmentum.project.domain.repository.TagRepository

class TagRepositoryImpl(
    private val api: TagApi
) : TagRepository {
    private val _tags = MutableStateFlow<List<Tag>>(emptyList())

    override suspend fun getAllTags(): Result<List<Tag>> {
        return when (val result = api.getAllTags()) {
            is ApiResult.Success -> {
                val tags = result.data.map { it.toDomain() }
                _tags.value = tags
                Result.success(tags)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun getTagById(tagId: Long): Result<Tag> {
        return when (val result = api.getTagById(tagId)) {
            is ApiResult.Success -> Result.success(result.data.toDomain())
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun createTag(name: String): Result<Unit> {
        return when (val result = api.createTag(TagRequest(name))) {
            is ApiResult.Success -> {
                getAllTags() // Refresh tags list
                Result.success(Unit)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun updateTag(tagId: Long, name: String): Result<Unit> {
        return when (val result = api.updateTag(tagId, TagRequest(name))) {
            is ApiResult.Success -> {
                getAllTags() // Refresh tags list
                Result.success(Unit)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override suspend fun deleteTag(tagId: Long): Result<Unit> {
        return when (val result = api.deleteTag(tagId)) {
            is ApiResult.Success -> {
                _tags.value = _tags.value.filter { it.id != tagId }
                Result.success(Unit)
            }
            is ApiResult.Error -> Result.failure(Exception(result.message))
        }
    }

    override fun observeTags(): Flow<List<Tag>> = _tags
}