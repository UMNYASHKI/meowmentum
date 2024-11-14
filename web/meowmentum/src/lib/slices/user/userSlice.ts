import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { User } from '@/lib/slices/user/userDtos';
import { RootState } from '@/lib/store';

const initialState: User = {
  id: '',
  name: '',
  email: '',
};

const name = 'user';

export const userSlice = createSlice({
  name: name,
  initialState,
  reducers: {
    setUser(state, action: PayloadAction<User>) {
      state = action.payload;
    },
  },
});

export const { setUser } = userSlice.actions;

export const selectUser = (state: RootState) => state[name];
