package org.meowmentum.project.di

import TaskRepositoryImpl
import org.koin.dsl.koinApplication
import org.koin.dsl.module
import org.meowmentum.project.data.remote.NetworkModule
import org.meowmentum.project.data.remote.auth.AuthApi
import org.meowmentum.project.data.remote.auth.AuthApiImpl
import org.meowmentum.project.data.local.AuthTokenStorage
import org.meowmentum.project.data.local.createAuthTokenStorage
import org.meowmentum.project.data.remote.tag.TagApi
import org.meowmentum.project.data.remote.tag.TagApiImpl
import org.meowmentum.project.data.remote.task.TaskApi
import org.meowmentum.project.data.remote.task.TaskApiImpl
import org.meowmentum.project.data.remote.timer.TimerApi
import org.meowmentum.project.data.remote.timer.TimerApiImpl
import org.meowmentum.project.data.repository.AuthRepositoryImpl
import org.meowmentum.project.data.repository.TagRepositoryImpl
import org.meowmentum.project.data.repository.TimerRepositoryImpl
import org.meowmentum.project.domain.repository.AuthRepository
import org.meowmentum.project.domain.repository.TagRepository
import org.meowmentum.project.domain.repository.TaskRepository
import org.meowmentum.project.domain.repository.TimerRepository
import org.meowmentum.project.ui.screens.auth.login.LoginViewModel
import org.meowmentum.project.ui.screens.auth.register.RegisterViewModel
import org.meowmentum.project.ui.screens.auth.forgotpassword.ForgotPasswordViewModel
import org.meowmentum.project.ui.screens.auth.resetpassword.ResetPasswordViewModel
import org.meowmentum.project.ui.screens.profile.ProfileViewModel
import org.meowmentum.project.ui.screens.task.create.CreateTaskViewModel
import org.meowmentum.project.ui.screens.task.edit.EditTaskViewModel
import org.meowmentum.project.ui.screens.task.list.TaskListViewModel
import org.meowmentum.project.ui.screens.timer.TimerViewModel
import org.meowmentum.project.viewmodel.AppViewModel

fun appModule() = module {
    // Storage
    single<AuthTokenStorage> { createAuthTokenStorage() }

    // Network
    single { NetworkModule.provideHttpClient(get()) }

    // API
    single<AuthApi> { AuthApiImpl(get()) }
    single<TaskApi> { TaskApiImpl(get()) }
    single<TagApi> { TagApiImpl(get()) }
    single<TimerApi> { TimerApiImpl(get()) }

    // Repository
    single<AuthRepository> { AuthRepositoryImpl(get(), get()) }
    single<TaskRepository> { TaskRepositoryImpl(get()) }
    single<TagRepository> { TagRepositoryImpl(get()) }
    single<TimerRepository> { TimerRepositoryImpl(get()) }

    // Task dependencies
    single<TaskApi> { TaskApiImpl(get()) }
    single<TaskRepository> { TaskRepositoryImpl(get()) }


    // ViewModels
    factory { LoginViewModel(get()) }
    factory { RegisterViewModel(get()) }
    factory { ForgotPasswordViewModel(get()) }
    factory { ResetPasswordViewModel(get()) }
    factory { TaskListViewModel(get(), get()) } // Now includes TagRepository for tag management
    factory { CreateTaskViewModel(get(), get()) }
    factory { EditTaskViewModel(get(), get()) }
    factory { TimerViewModel(get(), get()) }
//    factory { TagManagementViewModel(get()) }
    factory { AppViewModel(get()) }
    factory { ProfileViewModel(get(), get()) }
}

fun koinConfiguration() = koinApplication {
    appModule()
}