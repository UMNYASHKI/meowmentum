import { configureStore, createListenerMiddleware } from '@reduxjs/toolkit';
import { authApi } from '@/lib/services/auth/authApi';
import { userSlice } from '@/lib/slices/user/userSlice';
import { appSlice } from '@/lib/slices/app/appSlice';

const listenerMiddleware = createListenerMiddleware();

export const store = configureStore({
  reducer: {
    [authApi.reducerPath]: authApi.reducer,
    [userSlice.name]: userSlice.reducer,
    [appSlice.name]: appSlice.reducer,
  },
  devTools: process.env.NODE_ENV !== 'production',
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .prepend(listenerMiddleware.middleware)
      .concat(authApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
