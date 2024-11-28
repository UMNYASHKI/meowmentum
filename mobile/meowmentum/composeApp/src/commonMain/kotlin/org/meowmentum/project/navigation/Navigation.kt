package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import cafe.adriel.voyager.navigator.Navigator
import cafe.adriel.voyager.transitions.SlideTransition
import org.koin.compose.getKoin
import org.koin.compose.koinInject
import org.koin.core.annotation.KoinInternalApi
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.ui.screens.auth.login.LoginScreen
import org.meowmentum.project.ui.screens.home.HomeScreen

@OptIn(KoinInternalApi::class)
@Composable
fun AppNavigation() {
    val authRepository: AuthRepository = koinInject()
    val isLoggedIn by authRepository.isUserLoggedIn().collectAsState(initial = false)

    LaunchedEffect(Unit) {
        println("Initial login state: $isLoggedIn")
    }

    Navigator(
        screen = if (isLoggedIn) HomeScreen() else LoginScreen()
    ) { navigator ->
        SlideTransition(navigator)
    }
}