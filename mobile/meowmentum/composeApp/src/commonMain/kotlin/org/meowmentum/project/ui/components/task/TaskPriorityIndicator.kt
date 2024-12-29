package org.meowmentum.project.ui.components.task

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material.icons.outlined.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import kotlinx.datetime.Instant
import kotlinx.datetime.TimeZone
import kotlinx.datetime.toLocalDateTime
import org.meowmentum.project.domain.model.Tag

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskPriorityIndicator(
    priority: Int?,
    modifier: Modifier = Modifier
) {
    if (priority != null) {
        Icon(
            imageVector = Icons.Outlined.Flag,
            contentDescription = "Priority $priority",
            tint = when (priority) {
                1 -> MaterialTheme.colorScheme.error
                2 -> MaterialTheme.colorScheme.tertiary
                3 -> MaterialTheme.colorScheme.primary
                else -> MaterialTheme.colorScheme.outline
            },
            modifier = modifier.size(16.dp)
        )
    }
}