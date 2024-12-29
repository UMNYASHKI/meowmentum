package org.meowmentum.project.ui.components.navigation

import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier

enum class NavigationItem(val route: String) {
    TASKS("tasks"),
    TIMER("timer"),
    PROFILE("profile")
}

@Composable
fun AppBottomNavigation(
    currentRoute: String,
    onNavigate: (NavigationItem) -> Unit,
    modifier: Modifier = Modifier
) {
    NavigationBar(
        modifier = modifier,
        containerColor = MaterialTheme.colorScheme.background
    ) {
        NavigationBarItem(
            icon = { Icon(Icons.Default.Edit, contentDescription = "Tasks") },
            label = { Text("Tasks") },
            selected = currentRoute == NavigationItem.TASKS.route,
            onClick = { onNavigate(NavigationItem.TASKS) }
        )
        NavigationBarItem(
            icon = { Icon(Icons.Default.Timer, contentDescription = "Timer") },
            label = { Text("Timer") },
            selected = currentRoute == NavigationItem.TIMER.route,
            onClick = { onNavigate(NavigationItem.TIMER) }
        )
        NavigationBarItem(
            icon = { Icon(Icons.Default.Person, contentDescription = "Profile") },
            label = { Text("Profile") },
            selected = currentRoute == NavigationItem.PROFILE.route,
            onClick = { onNavigate(NavigationItem.PROFILE) }
        )
    }
}