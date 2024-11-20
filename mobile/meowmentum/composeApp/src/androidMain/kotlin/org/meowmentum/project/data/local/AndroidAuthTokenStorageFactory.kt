package org.meowmentum.project.data.local

import android.content.Context

lateinit var appContext: Context

actual fun createAuthTokenStorage(): AuthTokenStorage = AndroidAuthTokenStorage(appContext)