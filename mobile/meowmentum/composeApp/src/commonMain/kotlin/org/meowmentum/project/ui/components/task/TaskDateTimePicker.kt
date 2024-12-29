package org.meowmentum.project.ui.components.task

import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.*
import kotlinx.datetime.Clock
import kotlinx.datetime.Instant
import kotlinx.datetime.TimeZone
import kotlinx.datetime.atStartOfDayIn
import kotlinx.datetime.toLocalDateTime

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskDateTimePicker(
    selectedDate: Instant?,
    onDateSelected: (Instant?) -> Unit,
    onDismiss: () -> Unit
) {
    var showDatePicker by remember { mutableStateOf(true) }

    DatePickerDialog(
        onDismissRequest = onDismiss,
        confirmButton = {
            TextButton(onClick = {
                val now = Clock.System.now()
                val selectedDateTime = now.toLocalDateTime(TimeZone.currentSystemDefault())
                    .date.atStartOfDayIn(TimeZone.currentSystemDefault())
                onDateSelected(selectedDateTime)
                onDismiss()
            }) {
                Text("OK")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Cancel")
            }
        }
    ) {
        DatePicker(
            state = rememberDatePickerState(
                initialSelectedDateMillis = selectedDate?.toEpochMilliseconds()
            )
        )
    }
}