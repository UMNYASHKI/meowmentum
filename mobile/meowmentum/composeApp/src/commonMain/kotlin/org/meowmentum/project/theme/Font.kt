package org.meowmentum.project.theme

import androidx.compose.runtime.Composable
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import org.jetbrains.compose.resources.Font
import meowmentum.composeapp.generated.resources.Res
import meowmentum.composeapp.generated.resources.albertsans_bold
import meowmentum.composeapp.generated.resources.albertsans_semibold

@Composable
fun AlbertSansFontFamily() = FontFamily(
    Font(Res.font.albertsans_bold, weight = FontWeight.Bold),
    Font(Res.font.albertsans_semibold, weight = FontWeight.Normal)
)