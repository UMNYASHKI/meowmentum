package org.meowmentum.project.di

import io.ktor.client.*
import org.koin.core.module.Module
import org.koin.dsl.koinApplication
import org.koin.dsl.module
import org.meowmentum.project.data.remote.NetworkModule
import org.meowmentum.project.data.remote.AuthApi
import org.meowmentum.project.data.remote.AuthApiImpl
import org.meowmentum.project.data.local.AuthTokenStorage
import org.meowmentum.project.data.local.createAuthTokenStorage
import org.meowmentum.project.data.repository.AuthRepositoryImpl
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.ui.screens.auth.login.LoginViewModel
import org.meowmentum.project.ui.screens.auth.register.RegisterViewModel
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordViewModel
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordViewModel

fun appModule(): Module = module {
    // Network Client
    single<HttpClient> { NetworkModule.provideHttpClient() }

    // API Layer
    single<AuthApi> {
        AuthApiImpl(
            client = get()
        )
    }

    // Storage Layer
    single<AuthTokenStorage> {
        createAuthTokenStorage()
    }

    // Repository Layer
    single<AuthRepository> {
        AuthRepositoryImpl(
            api = get(),
            tokenStorage = get()
        )
    }

    // ViewModels
    factory { LoginViewModel(get()) }
    factory { RegisterViewModel(get()) }
    factory { ForgotPasswordViewModel(get()) }
    factory { ResetPasswordViewModel(get()) }
}

fun koinConfiguration() = koinApplication {
    appModule()
}