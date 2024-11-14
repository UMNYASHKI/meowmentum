import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { PopupMessage } from '@/lib/slices/app/appDtos';
import { RootState } from '@/lib/store';

interface AppInitialState {
  popupMessage: PopupMessage;
}
const initialState: AppInitialState = {
  popupMessage: {
    message: '',
    isVisible: false,
    type: null,
  },
};

const name = 'app';

export const appSlice = createSlice({
  name: name,
  initialState,
  reducers: {
    setPopupMessage(
      state,
      action: PayloadAction<AppInitialState['popupMessage']>
    ) {
      state.popupMessage = action.payload;
    },
    hidePopupMessage(state) {
      state.popupMessage.isVisible = false;
    },
  },
});

export const { setPopupMessage, hidePopupMessage } = appSlice.actions;

export const selectPopupMessage = (state: RootState) =>
  state[name].popupMessage;
