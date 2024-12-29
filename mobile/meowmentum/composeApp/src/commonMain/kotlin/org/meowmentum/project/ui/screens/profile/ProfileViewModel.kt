package org.meowmentum.project.ui.screens.profile

import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import moe.tlaster.precompose.viewmodel.ViewModel
import moe.tlaster.precompose.viewmodel.viewModelScope
import org.meowmentum.project.domain.model.*
import org.meowmentum.project.domain.repository.*

data class ProfileState(
    val user: User? = null,
    val tags: List<Tag> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
    val newTagName: String = "",
    val editingTag: Tag? = null
)

class ProfileViewModel(
    private val authRepository: AuthRepository,
    private val tagRepository: TagRepository
) : ViewModel() {
    private val _state = MutableStateFlow(ProfileState())
    val state = _state.asStateFlow()

    init {
        viewModelScope.launch {
            loadUserAndTags()
        }
    }

    private val userFlow = authRepository.getCurrentUser()

    init {
        viewModelScope.launch {
            userFlow.collect { user ->
                _state.update { it.copy(user = user) }
            }
        }
    }

    private suspend fun loadUserAndTags() {
        _state.update { it.copy(isLoading = true) }

        authRepository.getCurrentUser().collectLatest { user ->
            _state.update { it.copy(user = user) }
        }

        tagRepository.getAllTags()
            .onSuccess { tags -> _state.update { it.copy(tags = tags) } }
            .onFailure { error -> _state.update { it.copy(error = error.message) } }

        _state.update { it.copy(isLoading = false) }
    }

    fun updateNewTagName(name: String) {
        _state.update { it.copy(newTagName = name) }
    }

    fun createTag() {
        val name = _state.value.newTagName.trim()
        if (name.isBlank()) return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            tagRepository.createTag(name)
                .onSuccess { loadUserAndTags() }
                .onFailure { error -> _state.update { it.copy(error = error.message) } }

            _state.update {
                it.copy(
                    isLoading = false,
                    newTagName = ""
                )
            }
        }
    }

    fun startEditingTag(tag: Tag) {
        _state.update {
            it.copy(
                editingTag = tag,
                newTagName = tag.name
            )
        }
    }

    fun updateTag() {
        val tag = _state.value.editingTag ?: return
        val newName = _state.value.newTagName.trim()
        if (newName.isBlank()) return

        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            tagRepository.updateTag(tag.id, newName)
                .onSuccess {
                    loadUserAndTags()
                    _state.update { it.copy(editingTag = null, newTagName = "") }
                }
                .onFailure { error -> _state.update { it.copy(error = error.message) } }

            _state.update { it.copy(isLoading = false) }
        }
    }

    fun deleteTag(tagId: Long) {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }

            tagRepository.deleteTag(tagId)
                .onSuccess { loadUserAndTags() }
                .onFailure { error -> _state.update { it.copy(error = error.message) } }

            _state.update { it.copy(isLoading = false) }
        }
    }

    fun logout() {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true) }
            authRepository.logout()
            _state.update { it.copy(isLoading = false) }
        }
    }
}