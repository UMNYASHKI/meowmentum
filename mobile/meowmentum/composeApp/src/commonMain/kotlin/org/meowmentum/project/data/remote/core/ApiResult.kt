package org.meowmentum.project.data.remote.core

import io.ktor.client.call.*
import io.ktor.client.plugins.*
import io.ktor.client.statement.*
import io.ktor.http.*
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

sealed class ApiResult<out T> {
    data class Success<T>(val data: T) : ApiResult<T>()
    data class Error(val code: Int? = null, val message: String) : ApiResult<Nothing>()
}

@Serializable
data class ApiErrorResponse(
    @SerialName("error") val error: String? = null,
    @SerialName("message") val message: String? = null,
    @SerialName("errorMessage") val errorMessage: String? = null
)

suspend inline fun <reified T> handleApiResponse(
    crossinline apiCall: suspend () -> HttpResponse
): ApiResult<T> {
    return try {
        val response = apiCall()

        when {
            response.status.isSuccess() -> {
                val body = response.body<T>()
                ApiResult.Success(body)
            }
            else -> {
                val errorBody = try {
                    response.body<ApiErrorResponse>()
                } catch (e: Exception) {
                    null
                }

                ApiResult.Error(
                    code = response.status.value,
                    message = errorBody?.message
                        ?: errorBody?.errorMessage
                        ?: errorBody?.error
                        ?: response.status.description
                )
            }
        }
    } catch (e: Exception) {
        val message = when (e) {
            is ResponseException -> when (e.response.status.value) {
                401 -> "Unauthorized. Please log in again."
                403 -> "Access denied"
                404 -> "Resource not found"
                in 500..599 -> "Server error. Please try again later."
                else -> e.response.status.description
            }
            is ClientRequestException -> "Invalid request"
            is ServerResponseException -> "Server error. Please try again later."
            else -> when (e) {
                is java.net.UnknownHostException,
                is java.net.SocketTimeoutException,
                is java.io.IOException -> "Network error. Please check your connection."
                else -> e.message ?: "Unknown error occurred"
            }
        }
        ApiResult.Error(message = message)
    }
}

// Extension function to make error handling more concise
suspend inline fun <reified T> ApiResult<T>.onError(
    crossinline action: suspend (String) -> Unit
): ApiResult<T> {
    if (this is ApiResult.Error) {
        action(message)
    }
    return this
}

// Extension function to make success handling more concise
suspend inline fun <reified T> ApiResult<T>.onSuccess(
    crossinline action: suspend (T) -> Unit
): ApiResult<T> {
    if (this is ApiResult.Success) {
        action(data)
    }
    return this
}