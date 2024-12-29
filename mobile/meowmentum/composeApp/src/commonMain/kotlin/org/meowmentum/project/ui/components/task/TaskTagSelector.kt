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
import org.meowmentum.project.domain.model.Tag


@Composable
fun TaskTagSelector(
    availableTags: List<Tag>,
    selectedTagIds: List<Long>,
    onTagSelectionChanged: (List<Long>) -> Unit,
    modifier: Modifier = Modifier
) {
    var showDialog by remember { mutableStateOf(false) }

    OutlinedButton(
        onClick = { showDialog = true },
        modifier = modifier,
        shape = RoundedCornerShape(12.dp)
    ) {
        Icon(Icons.Default.Tag, contentDescription = null)
        Spacer(Modifier.width(8.dp))
        Text(
            if (selectedTagIds.isEmpty()) "Add Tags"
            else "${selectedTagIds.size} Tags Selected"
        )
    }

    if (showDialog) {
        AlertDialog(
            onDismissRequest = { showDialog = false },
            title = { Text("Select Tags") },
            text = {
                Column {
                    availableTags.forEach { tag ->
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(vertical = 4.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Checkbox(
                                checked = tag.id in selectedTagIds,
                                onCheckedChange = { isChecked ->
                                    val newSelection = if (isChecked) {
                                        selectedTagIds + tag.id
                                    } else {
                                        selectedTagIds - tag.id
                                    }
                                    onTagSelectionChanged(newSelection)
                                }
                            )
                            Spacer(Modifier.width(8.dp))
                            Text(tag.name)
                        }
                    }
                }
            },
            confirmButton = {
                TextButton(onClick = { showDialog = false }) {
                    Text("Done")
                }
            },
            dismissButton = {
                TextButton(
                    onClick = {
                        onTagSelectionChanged(emptyList())
                        showDialog = false
                    }
                ) {
                    Text("Clear All")
                }
            }
        )
    }
}