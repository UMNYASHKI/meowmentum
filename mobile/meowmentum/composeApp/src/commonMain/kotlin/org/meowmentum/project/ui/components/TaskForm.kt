package org.meowmentum.project.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import org.meowmentum.project.domain.model.Task

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskForm(
    title: String,
    initialTask: Task? = null,
    isLoading: Boolean = false,
    onSave: (String, String?, String?, Int, String?) -> Unit,
    onBack: () -> Unit
) {
    var taskTitle by remember { mutableStateOf(initialTask?.title ?: "") }
    var description by remember { mutableStateOf(initialTask?.description ?: "") }
    var dueDate by remember { mutableStateOf(initialTask?.dueDate) }
    var priority by remember { mutableStateOf(initialTask?.priority ?: 0) }
    var tag by remember { mutableStateOf(initialTask?.tag) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(title) },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, "Back")
                    }
                }
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            OutlinedTextField(
                value = taskTitle,
                onValueChange = { taskTitle = it },
                label = { Text("Task Name") },
                modifier = Modifier.fillMaxWidth()
            )

            Spacer(modifier = Modifier.height(16.dp))

            OutlinedTextField(
                value = description,
                onValueChange = { description = it },
                label = { Text("Description") },
                modifier = Modifier.fillMaxWidth()
            )

            Spacer(modifier = Modifier.height(16.dp))

            // Date picker button
            DatePickerButton(
                selectedDate = dueDate,
                onDateSelected = { dueDate = it }
            )

            Spacer(modifier = Modifier.height(16.dp))

            // Priority selector
            PrioritySelector(
                selectedPriority = priority,
                onPrioritySelected = { priority = it }
            )

            Spacer(modifier = Modifier.height(16.dp))

            // Tag selector
            TagSelector(
                selectedTag = tag,
                onTagSelected = { tag = it }
            )

            Spacer(modifier = Modifier.weight(1f))

            Button(
                onClick = { onSave(taskTitle, description, dueDate, priority, tag) },
                modifier = Modifier.fillMaxWidth(),
                enabled = taskTitle.isNotBlank() && !isLoading
            ) {
                Text("Save")
            }
        }
    }
}