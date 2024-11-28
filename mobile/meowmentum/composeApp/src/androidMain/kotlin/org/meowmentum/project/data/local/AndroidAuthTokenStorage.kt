package org.meowmentum.project.data.local

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.map

private val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "auth_tokens")

class AndroidAuthTokenStorage(
    private val context: Context
) : AuthTokenStorage {
    private object PreferencesKeys {
        val ACCESS_TOKEN = stringPreferencesKey("access_token")
        val REFRESH_TOKEN = stringPreferencesKey("refresh_token")
    }

    override suspend fun saveTokens(accessToken: String, refreshToken: String) {
        context.dataStore.edit { preferences ->
            preferences[PreferencesKeys.ACCESS_TOKEN] = accessToken
            preferences[PreferencesKeys.REFRESH_TOKEN] = refreshToken
        }
    }

    override suspend fun getAccessToken(): String? {
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.ACCESS_TOKEN]
        }.firstOrNull()
    }

    override suspend fun getRefreshToken(): String? {
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.REFRESH_TOKEN]
        }.firstOrNull()
    }

    override fun getAccessTokenFlow(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.ACCESS_TOKEN]
        }
    }

    override fun getRefreshTokenFlow(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[PreferencesKeys.REFRESH_TOKEN]
        }
    }

    override suspend fun clearTokens() {
        context.dataStore.edit { preferences ->
            preferences.remove(PreferencesKeys.ACCESS_TOKEN)
            preferences.remove(PreferencesKeys.REFRESH_TOKEN)
        }
    }

    override suspend fun updateAccessToken(newToken: String) {
        context.dataStore.edit { preferences ->
            preferences[PreferencesKeys.ACCESS_TOKEN] = newToken
        }
    }
}