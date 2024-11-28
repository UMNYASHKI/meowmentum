import { configureStore, createListenerMiddleware } from '@reduxjs/toolkit';
import { authApi } from '@/lib/services/auth/authApi';
import { userSlice } from '@/lib/slices/user/userSlice';
import { appSlice } from '@/lib/slices/app/appSlice';
import { tagSlice } from '@/lib/slices/tags/tagsSlice';
import { tagApi } from '@services/tags/tagApi';

const listenerMiddleware = createListenerMiddleware();

export const store = configureStore({
  reducer: {
    [authApi.reducerPath]: authApi.reducer,
    [tagApi.reducerPath]: tagApi.reducer,
    [userSlice.name]: userSlice.reducer,
    [appSlice.name]: appSlice.reducer,
    [tagSlice.name]: tagSlice.reducer,
  },
  devTools: process.env.NODE_ENV !== 'production',
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .prepend(listenerMiddleware.middleware)
      .concat(authApi.middleware)
      .concat(tagApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
