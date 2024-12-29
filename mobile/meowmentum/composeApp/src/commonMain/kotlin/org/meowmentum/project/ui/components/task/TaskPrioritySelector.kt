package org.meowmentum.project.ui.components.task

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun TaskPrioritySelector(
    selectedPriority: Int?,
    onPrioritySelected: (Int?) -> Unit,
    modifier: Modifier = Modifier
) {
    var showDialog by remember { mutableStateOf(false) }

    OutlinedButton(
        onClick = { showDialog = true },
        modifier = modifier,
        shape = RoundedCornerShape(12.dp)
    ) {
        Icon(
            imageVector = Icons.Default.Flag,
            contentDescription = null,
            tint = when (selectedPriority) {
                1 -> MaterialTheme.colorScheme.error
                2 -> MaterialTheme.colorScheme.tertiary
                3 -> MaterialTheme.colorScheme.primary
                else -> MaterialTheme.colorScheme.onSurface
            }
        )
        Spacer(Modifier.width(8.dp))
        Text(
            when (selectedPriority) {
                1 -> "High Priority"
                2 -> "Medium Priority"
                3 -> "Low Priority"
                else -> "Set Priority"
            }
        )
    }

    if (showDialog) {
        AlertDialog(
            onDismissRequest = { showDialog = false },
            title = { Text("Select Priority") },
            text = {
                Column {
                    listOf(
                        Triple(1, "High Priority", MaterialTheme.colorScheme.error),
                        Triple(2, "Medium Priority", MaterialTheme.colorScheme.tertiary),
                        Triple(3, "Low Priority", MaterialTheme.colorScheme.primary)
                    ).forEach { (priority, label, color) ->
                        TextButton(
                            onClick = {
                                onPrioritySelected(priority)
                                showDialog = false
                            },
                            modifier = Modifier.fillMaxWidth()
                        ) {
                            Row(
                                horizontalArrangement = Arrangement.spacedBy(8.dp),
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Icon(
                                    Icons.Default.Flag,
                                    contentDescription = null,
                                    tint = color
                                )
                                Text(label)
                            }
                        }
                    }
                }
            },
            confirmButton = {
                TextButton(
                    onClick = {
                        onPrioritySelected(null)
                        showDialog = false
                    }
                ) {
                    Text("Clear Priority")
                }
            },
            dismissButton = {
                TextButton(onClick = { showDialog = false }) {
                    Text("Cancel")
                }
            }
        )
    }
}