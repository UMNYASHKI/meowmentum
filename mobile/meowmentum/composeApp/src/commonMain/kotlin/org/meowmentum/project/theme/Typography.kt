package org.meowmentum.project.theme

import androidx.compose.material3.Typography
import androidx.compose.runtime.Composable
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.sp

@Composable
fun AlbertSansTypography() = Typography(
    // headline 32, lineHeight 36 weight 900
    // lable 16, lineHeight 20 weight 700
    // body-small 14, lineHeight 20 weight 400
    // body-medium 16, lineHeight 24 weight 400
    // body-large 20, lineHeight 28 weight 400
    // headline-small 24, lineHeight 28 weight 500
    // label-small 14, lineHeight 20 weight 600
    // lable-large 24, lineHeight 20 weight 600

    headlineLarge = TextStyle(
        fontWeight = FontWeight.W900,
        fontSize = 32.sp,
        lineHeight = 36.sp
    ),
    headlineSmall = TextStyle(
        fontWeight = FontWeight.W500,
        fontSize = 24.sp,
        lineHeight = 28.sp
    ),
    labelMedium = TextStyle(
        fontWeight = FontWeight.W700,
        fontSize = 16.sp,
        lineHeight = 20.sp
    ),
    labelSmall = TextStyle(
        fontWeight = FontWeight.W600,
        fontSize = 14.sp,
        lineHeight = 20.sp
    ),
    labelLarge = TextStyle(
        fontWeight = FontWeight.W600,
        fontSize = 24.sp,
        lineHeight = 20.sp
    ),
    bodySmall = TextStyle(
        fontWeight = FontWeight.W400,
        fontSize = 14.sp,
        lineHeight = 20.sp
    ),
    bodyMedium = TextStyle(
        fontWeight = FontWeight.W400,
        fontSize = 16.sp,
        lineHeight = 24.sp
    ),
    bodyLarge = TextStyle(
        fontWeight = FontWeight.W400,
        fontSize = 20.sp,
        lineHeight = 28.sp
    )
)