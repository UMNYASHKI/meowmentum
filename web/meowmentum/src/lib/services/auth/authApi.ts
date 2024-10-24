import { createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@/lib/services/helpers/baseAuthQuery';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
} from '@/lib/services/auth/authDtos';
import { useAuth } from '@/lib/providers/authProvider';

export const authApi = createApi({
  reducerPath: 'apiAuth',
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    register: builder.mutation<void, RegisterRequest>({
      query: (credentials) => ({
        url: '/auth/register',
        method: 'POST',
        body: credentials,
      }),
    }),
    login: builder.mutation<LoginResponse, LoginRequest>({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
      onQueryStarted: (arg, { queryFulfilled }) => {
        queryFulfilled.then((res) => {
          const token = res.data.token;
          if (token) {
            const { login } = useAuth();
            login(token);
          }
        });
      },
    }),
  }),
});

export const { useRegisterMutation, useLoginMutation } = authApi;
