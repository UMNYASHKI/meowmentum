package org.meowmentum.project.ui.components.timer

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import org.meowmentum.project.domain.model.Task
import kotlin.time.Duration.Companion.milliseconds

@Composable
fun TaskList(
    tasks: List<Task>,
    onTaskSelect: (Task) -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier.fillMaxWidth(),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        tasks.forEach { task ->
            TaskListItem(
                task = task,
                onTaskSelect = { onTaskSelect(task) }
            )
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun TaskListItem(
    task: Task,
    onTaskSelect: () -> Unit
) {
    Card(
        onClick = onTaskSelect,
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        )
    ) {
        Row(
            modifier = Modifier
                .padding(16.dp)
                .fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column {
                Text(
                    text = task.title,
                    style = MaterialTheme.typography.titleMedium
                )
//                Text(
//                    text = task.description,
//                    style = MaterialTheme.typography.bodyMedium,
//                    color = MaterialTheme.colorScheme.onSurfaceVariant
//                )
            }
            Icon(
                Icons.Default.KeyboardArrowRight,
                contentDescription = "Select Task",
                tint = MaterialTheme.colorScheme.onSurfaceVariant
            )
        }
    }
}