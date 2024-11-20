package org.meowmentum.project

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent


class MeowmentumApplication : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MainApp()
        }
    }
}
