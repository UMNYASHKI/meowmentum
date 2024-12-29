package org.meowmentum.project.ui.components.task

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import org.meowmentum.project.domain.model.Tag

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskTagDisplay(
    tag: Tag?,
    modifier: Modifier = Modifier
) {
    if (tag != null) {
        Row(
            horizontalArrangement = Arrangement.spacedBy(4.dp),
            verticalAlignment = Alignment.CenterVertically,
            modifier = modifier
        ) {
            Icon(
                Icons.Filled.Tag,
                contentDescription = null,
                modifier = Modifier.size(16.dp),
                tint = MaterialTheme.colorScheme.outline
            )
            Text(
                text = tag.name,
                style = MaterialTheme.typography.bodySmall,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )
        }
    }
}