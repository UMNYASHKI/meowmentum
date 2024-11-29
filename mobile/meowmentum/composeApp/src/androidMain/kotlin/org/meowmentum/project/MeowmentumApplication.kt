package org.meowmentum.project

import android.app.Application
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import org.koin.android.ext.koin.androidContext
import org.koin.android.ext.koin.androidLogger
import org.koin.androidx.compose.KoinAndroidContext
import org.koin.core.context.startKoin
import org.koin.core.logger.Level
import org.meowmentum.project.data.local.appContext
import org.meowmentum.project.di.appModule

class MeowApp : Application() {
    override fun onCreate() {
        super.onCreate()
        appContext = applicationContext
        startKoin {
            androidContext(this@MeowApp)
            androidLogger(Level.DEBUG)
            modules(appModule())
        }
    }
}

class MeowmentumApplication : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            KoinAndroidContext {
                MainApp()
            }
        }
    }
}
