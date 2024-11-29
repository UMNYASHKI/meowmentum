package org.meowmentum.project

import androidx.compose.runtime.Composable
import org.koin.compose.KoinApplication
import org.meowmentum.project.di.koinConfiguration
import org.meowmentum.project.navigation.AppNavigation
import org.meowmentum.project.theme.MainTheme

@Composable
fun MainApp() {
    //KoinApplication(::koinConfiguration){
        MainTheme {
            AppNavigation()
        }
    //}
}
