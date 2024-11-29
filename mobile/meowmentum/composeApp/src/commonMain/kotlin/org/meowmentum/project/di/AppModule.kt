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
import org.meowmentum.project.data.remote.TaskApi
import org.meowmentum.project.data.remote.TaskApiImpl
import org.meowmentum.project.data.repository.AuthRepositoryImpl
import org.meowmentum.project.data.repository.TaskRepositoryImpl
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.repository.TaskRepository
import org.meowmentum.project.ui.screens.auth.login.LoginViewModel
import org.meowmentum.project.ui.screens.auth.register.RegisterViewModel
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordViewModel
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordViewModel

fun appModule() = module {
    // Storage
    single<AuthTokenStorage> { createAuthTokenStorage() }

    // Network
    single { NetworkModule.provideHttpClient(get()) }

    // API
    single<AuthApi> { AuthApiImpl(get()) }

    // Repository
    single<AuthRepository> { AuthRepositoryImpl(get(), get()) }

    // Task dependencies
    single<TaskApi> { TaskApiImpl(get()) }
    single<TaskRepository> { TaskRepositoryImpl(get()) }

    // ViewModels
    factory { LoginViewModel(get()) }
    factory { RegisterViewModel(get()) }
    factory { ForgotPasswordViewModel(get()) }
    factory { ResetPasswordViewModel(get()) }
}

fun koinConfiguration() = koinApplication {
    appModule()
}