package org.meowmentum.project.data.remote

import io.ktor.client.*
import io.ktor.client.plugins.contentnegotiation.*
import io.ktor.client.plugins.logging.*
import io.ktor.serialization.kotlinx.json.*
import kotlinx.serialization.json.Json
import org.meowmentum.project.data.local.AuthTokenStorage

object NetworkModule {
    private val json = Json {
        ignoreUnknownKeys = true
        coerceInputValues = true
        isLenient = true
        prettyPrint = true
        encodeDefaults = true
    }

    fun provideHttpClient(tokenStorage: AuthTokenStorage): HttpClient {
        return HttpClient {
            install(ContentNegotiation) {
                json(json)
            }

            install(TokenAuthPlugin) {
                this.tokenStorage = tokenStorage
            }

            install(Logging) {
                level = LogLevel.ALL
            }
        }
    }
}