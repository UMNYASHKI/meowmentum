package org.meowmentum.project.ui.components.task

import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskStatusIndicator(
    isCompleted: Boolean,
    onStatusChange: (Boolean) -> Unit,
    modifier: Modifier = Modifier
) {
    Checkbox(
        checked = isCompleted,
        onCheckedChange = onStatusChange,
        colors = CheckboxDefaults.colors(
            checkedColor = MaterialTheme.colorScheme.primary,
            uncheckedColor = MaterialTheme.colorScheme.outline
        ),
        modifier = modifier
    )
}