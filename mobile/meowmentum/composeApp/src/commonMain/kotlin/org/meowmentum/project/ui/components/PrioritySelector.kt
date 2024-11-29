package org.meowmentum.project.ui.components

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PrioritySelector(
    selectedPriority: Int,
    onPrioritySelected: (Int) -> Unit,
    modifier: Modifier = Modifier
) {
    var expanded by remember { mutableStateOf(false) }
    val priorities = listOf(
        0 to "No Priority",
        1 to "Priority 1",
        2 to "Priority 2",
        3 to "Priority 3"
    )

    val priorityColors = mapOf(
        1 to MaterialTheme.colorScheme.error,
        2 to Color(0xFFFFB347), // Orange
        3 to MaterialTheme.colorScheme.tertiary
    )

    ExposedDropdownMenuBox(
        expanded = expanded,
        onExpandedChange = { expanded = it },
        modifier = modifier
    ) {
        OutlinedTextField(
            value = priorities.find { it.first == selectedPriority }?.second ?: "Select Priority",
            onValueChange = {},
            readOnly = true,
            trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = expanded) },
            modifier = Modifier.menuAnchor()
        )

        ExposedDropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false }
        ) {
            priorities.forEach { (priority, label) ->
                DropdownMenuItem(
                    text = {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(8.dp)
                        ) {
                            if (priority > 0) {
                                Box(
                                    modifier = Modifier
                                        .size(12.dp)
                                        .background(
                                            priorityColors[priority] ?: MaterialTheme.colorScheme.secondary,
                                            shape = CircleShape
                                        )
                                )
                            }
                            Text(label)
                        }
                    },
                    onClick = {
                        onPrioritySelected(priority)
                        expanded = false
                    }
                )
            }
        }
    }
}