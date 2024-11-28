package org.meowmentum.project.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

@Composable
fun MainTheme(content: @Composable () -> Unit) {
    val colors = lightColorScheme(
        primary = PrimaryColor,
        secondary = SecondaryColor,
        tertiary = TeritaryColor,
        error = AccentColor,
        errorContainer = AccentSurfaceColor,
        background = BackgroundColor
    )
    val typography = AlbertSansTypography()
    MaterialTheme(
        colorScheme = colors,
        typography = typography,
        content = content
    )
}