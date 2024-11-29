package org.meowmentum.project.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Tag

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TagSelector(
    selectedTag: String?,
    onTagSelected: (String) -> Unit,
    modifier: Modifier = Modifier
) {
    var expanded by remember { mutableStateOf(false) }
    val tags = listOf("School", "Work", "Personal")

    ExposedDropdownMenuBox(
        expanded = expanded,
        onExpandedChange = { expanded = it },
        modifier = modifier
    ) {
        OutlinedTextField(
            value = selectedTag ?: "Add Tag",
            onValueChange = {},
            readOnly = true,
            leadingIcon = {
                Icon(
                    imageVector = Icons.Default.Tag,
                    contentDescription = "Tag",
                    tint = MaterialTheme.colorScheme.primary
                )
            },
            trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = expanded) },
            modifier = Modifier.menuAnchor()
        )

        ExposedDropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false }
        ) {
            tags.forEach { tag ->
                DropdownMenuItem(
                    text = { Text(tag) },
                    onClick = {
                        onTagSelected(tag)
                        expanded = false
                    }
                )
            }
        }
    }
}