package org.meowmentum.project.theme

import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

@Composable
fun MainTheme(content: @Composable () -> Unit) {
    val colors = lightColorScheme(
        background = BackgroundColor,
        surface = PrimarySurfaceColor,
        onPrimary = PrimaryTextColor,
        onSecondary = SecondaryTextColor,
        onError = AccentColor,
        onErrorContainer = AccentSurfaceColor
    )
}