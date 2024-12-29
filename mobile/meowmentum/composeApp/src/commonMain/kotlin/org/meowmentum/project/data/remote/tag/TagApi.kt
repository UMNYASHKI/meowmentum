package org.meowmentum.project.data.remote.tag

import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult

interface TagApi {
    suspend fun getAllTags(): ApiResult<List<TagResponse>>
    suspend fun getTagById(tagId: Long): ApiResult<TagResponse>
    suspend fun createTag(request: TagRequest): ApiResult<Unit>
    suspend fun updateTag(tagId: Long, request: TagRequest): ApiResult<Unit>
    suspend fun deleteTag(tagId: Long): ApiResult<Unit>
}