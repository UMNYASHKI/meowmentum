package org.meowmentum.project

import androidx.compose.runtime.Composable
import org.meowmentum.project.navigation.AppNavigation
import org.meowmentum.project.theme.MainTheme

@Composable
fun MainApp() {
    MainTheme {
        AppNavigation()
    }
}