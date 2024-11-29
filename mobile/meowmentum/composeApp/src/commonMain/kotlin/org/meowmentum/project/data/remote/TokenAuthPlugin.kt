package org.meowmentum.project.data.remote

import io.ktor.client.plugins.api.createClientPlugin
import io.ktor.client.request.*
import org.meowmentum.project.data.local.AuthTokenStorage

class TokenAuthConfig {
    lateinit var tokenStorage: AuthTokenStorage
}

val TokenAuthPlugin =createClientPlugin("TokenAuthPlugin", ::TokenAuthConfig) {
    val tokenStorage = pluginConfig.tokenStorage

    onRequest { request, _ ->
        val token = tokenStorage.getAccessToken()
        token?.let {
            request.header("Authorization", "Bearer $it")
        }
    }
}