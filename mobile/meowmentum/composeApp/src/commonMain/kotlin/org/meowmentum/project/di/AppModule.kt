/*
package org.meowmentum.project.di

import org.koin.core.module.Module
import org.koin.dsl.module
import org.meowmentum.project.data.local.AuthTokenStorage
import org.meowmentum.project.data.remote.AuthApi
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordViewModel
import org.meowmentum.project.ui.screens.auth.login.LoginViewModel
import org.meowmentum.project.ui.screens.auth.register.RegisterViewModel
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordViewModel

fun appModule() = module {
    // Network
    single { provideHttpClient() }

    // API
    single<AuthApi> { AuthApiImpl(get()) }

    // Storage
    single<AuthTokenStorage> { createAuthTokenStorage() }

    // Repository
    single<AuthRepository> { AuthRepositoryImpl(get(), get()) }

    // ViewModels
    factory { LoginViewModel(get()) }
    factory { RegisterViewModel(get()) }
    factory { ForgotPasswordViewModel(get()) }
    factory { ResetPasswordViewModel(get()) }
}*/
