package org.meowmentum.project.di

import org.koin.dsl.module
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordViewModel
import org.meowmentum.project.ui.screens.auth.login.LoginViewModel
import org.meowmentum.project.ui.screens.auth.register.RegisterViewModel
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordViewModel


val viewModelModule = module {
    factory { LoginViewModel(get()) }
    factory { RegisterViewModel(get()) }
    factory { ForgotPasswordViewModel(get()) }
    factory { ResetPasswordViewModel(get()) }
}