package org.meowmentum.project.navigation

import androidx.compose.runtime.Composable
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

    println("isLoggedIn:")

    val koin = getKoin()

    println(koin.instanceRegistry.instances)

    val authRepository: AuthRepository = koinInject()

    println("DI AuthRepository resolved: $authRepository")

    val isLoggedIn by authRepository.isUserLoggedIn().collectAsState(initial = false)


    Navigator(
        screen = if (isLoggedIn) HomeScreen() else LoginScreen()
    ) { navigator ->
        SlideTransition(navigator)
    }
}