import { createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@/lib/services/helpers/baseAuthQuery';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  VerificationCodeRequest,
} from '@/lib/services/auth/authDtos';
import { useAuth } from '@/lib/providers/authProvider';

export const authApi = createApi({
  reducerPath: 'apiAuth',
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    register: builder.mutation<void, RegisterRequest>({
      query: (credentials) => ({
        url: '/Auth/register',
        method: 'POST',
        body: credentials,
      }),
    }),
    verifyOtp: builder.mutation<void, VerificationCodeRequest>({
      query: (credentials) => ({
        url: '/Auth/verify-otp',
        method: 'POST',
        body: credentials,
      }),
    }),
    login: builder.mutation<LoginResponse, LoginRequest>({
      query: (credentials) => ({
        url: '/Auth/login',
        method: 'POST',
        body: credentials,
      }),
    }),
  }),
});

export const { useRegisterMutation, useVerifyOtpMutation, useLoginMutation } =
  authApi;
