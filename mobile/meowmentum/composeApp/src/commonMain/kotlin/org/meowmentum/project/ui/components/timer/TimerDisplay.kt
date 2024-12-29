package org.meowmentum.project.ui.components.timer

import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.sp


@Composable
fun TimerDisplay(
    elapsedTimeMillis: Long,
    modifier: Modifier = Modifier
) {
    val hours = (elapsedTimeMillis / (1000 * 60 * 60)).toInt()
    val minutes = ((elapsedTimeMillis / (1000 * 60)) % 60).toInt()
    val seconds = ((elapsedTimeMillis / 1000) % 60).toInt()

    val displayText = if (hours > 0) {
        "%02d:%02d:%02d".format(hours, minutes, seconds)
    } else {
        "%02d:%02d".format(minutes, seconds)
    }

    Text(
        text = displayText,
        fontSize = 64.sp,
        fontWeight = FontWeight.Bold,
        textAlign = TextAlign.Center,
        color = MaterialTheme.colorScheme.onSurface,
        modifier = modifier
    )
}