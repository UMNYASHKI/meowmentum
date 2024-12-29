package org.meowmentum.project.ui.components.task

import androidx.compose.foundation.BorderStroke
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
import org.meowmentum.project.domain.model.Tag

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskListItem(
    title: String,
    isCompleted: Boolean,
    onStatusChange: (Boolean) -> Unit,
    onItemClick: () -> Unit,
    modifier: Modifier = Modifier,
    description: String? = null,
    date: Instant? = null,
    priority: Int? = null,
    tag: Tag? = null
) {
    Card(
        onClick = onItemClick,
        modifier = modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        ),
        border = BorderStroke(1.dp, MaterialTheme.colorScheme.onSurface.copy(alpha = 0.1f))
    ) {
        Row(
            modifier = Modifier
                .padding(16.dp)
                .fillMaxWidth(),
            verticalAlignment = Alignment.CenterVertically
        ) {
            TaskStatusIndicator(
                isCompleted = isCompleted,
                onStatusChange = onStatusChange
            )

            Column(
                modifier = Modifier
                    .weight(1f)
                    .padding(horizontal = 12.dp),
                verticalArrangement = Arrangement.spacedBy(4.dp)
            ) {
                Text(
                    text = title,
                    style = MaterialTheme.typography.bodyLarge,
                    color = if (isCompleted)
                        MaterialTheme.colorScheme.onSurfaceVariant
                    else
                        MaterialTheme.colorScheme.onSurface
                )

//                description?.let {
//                    Text(
//                        text = it,
//                        style = MaterialTheme.typography.bodyMedium,
//                        color = MaterialTheme.colorScheme.onSurfaceVariant,
//                        maxLines = 2,
//                        overflow = TextOverflow.Ellipsis
//                    )
//                }

                Row(
                    horizontalArrangement = Arrangement.spacedBy(8.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    TaskDateDisplay(date = date)
                    TaskPriorityIndicator(priority = priority)
                    TaskTagDisplay(tag = tag)
                }
            }

            IconButton(onClick = onItemClick) {
                Icon(
                    Icons.Default.ChevronRight,
                    contentDescription = "View Details",
                    tint = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}