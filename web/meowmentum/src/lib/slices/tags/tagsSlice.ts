import { ITag } from '@/common/tags';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '@/lib/store';

const initialState: ITag[] = [];

const name = 'tags';

export const tagSlice = createSlice({
  name: name,
  initialState,
  reducers: {
    setTags: (state, action: PayloadAction<ITag[]>) => {
      state = action.payload;
    },
    addTag: (state, action: PayloadAction<ITag>) => {
      state.push(action.payload);
    },
    removeTag: (state, action: PayloadAction<ITag>) => {},
  },
});

export const { addTag, removeTag } = tagSlice.actions;

export const selectTags = (state: RootState) => state[name];
