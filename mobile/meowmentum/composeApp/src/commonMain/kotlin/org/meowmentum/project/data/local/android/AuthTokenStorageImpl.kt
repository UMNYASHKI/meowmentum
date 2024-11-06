/*
package org.meowmentum.project.data.local.android

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import org.meowmentum.project.data.local.AuthTokenStorage

// Extension property for DataStore
private val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "auth_tokens")

class AndroidAuthTokenStorage(
    private val context: Context
) : AuthTokenStorage {
    private object PreferencesKeys {
        val AUTH_TOKEN = stringPreferencesKey("auth_token")
        val REFRESH_TOKEN = stringPreferencesKey("refresh_token")
    }

    override suspend fun saveTokens(token: String, refreshToken: String) {
        // DataStore provides a safe way to store key-value pairs asynchronously
        context.dataStore.edit { preferences ->
            // Store both tokens in preferences
            preferences[PreferencesKeys.AUTH_TOKEN] = token
            preferences[PreferencesKeys.REFRESH_TOKEN] = refreshToken
        }
    }

    override suspend fun getToken(): String? {
        // Get the auth token from preferences synchronously
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.AUTH_TOKEN]
        }.firstOrNull()
    }

    override suspend fun getRefreshToken(): String? {
        // Get the refresh token from preferences synchronously
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.REFRESH_TOKEN]
        }.firstOrNull()
    }

    override fun getTokenFlow(): Flow<String?> {
        // Return a Flow of auth token that updates whenever the token changes
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.AUTH_TOKEN]
        }
    }

    override fun getRefreshTokenFlow(): Flow<String?> {
        // Return a Flow of refresh token that updates whenever the token changes
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.REFRESH_TOKEN]
        }
    }

    override suspend fun clearTokens() {
        // Remove both tokens from preferences
        context.dataStore.edit { preferences ->
            preferences.remove(PreferencesKeys.AUTH_TOKEN)
            preferences.remove(PreferencesKeys.REFRESH_TOKEN)
        }
    }
}*/
