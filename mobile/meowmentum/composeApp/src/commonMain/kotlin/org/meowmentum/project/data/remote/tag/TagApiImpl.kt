package org.meowmentum.project.data.remote.tag

import io.ktor.client.*
import io.ktor.client.request.*
import io.ktor.http.*
import org.meowmentum.project.data.models.*
import org.meowmentum.project.data.remote.core.ApiResult
import org.meowmentum.project.data.remote.core.handleApiResponse

class TagApiImpl(
    private val client: HttpClient,
    private val baseUrl: String = "http://10.0.2.2:8080/api/core/api/tag"
) : TagApi {
    override suspend fun getAllTags(): ApiResult<List<TagResponse>> =
        handleApiResponse {
            client.get(baseUrl)
        }

    override suspend fun getTagById(tagId: Long): ApiResult<TagResponse> =
        handleApiResponse {
            client.get("$baseUrl/$tagId")
        }

    override suspend fun createTag(request: TagRequest): ApiResult<Unit> =
        handleApiResponse {
            client.post(baseUrl) {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun updateTag(tagId: Long, request: TagRequest): ApiResult<Unit> =
        handleApiResponse {
            client.put("$baseUrl/$tagId") {
                contentType(ContentType.Application.Json)
                setBody(request)
            }
        }

    override suspend fun deleteTag(tagId: Long): ApiResult<Unit> =
        handleApiResponse {
            client.delete("$baseUrl/$tagId")
        }
}