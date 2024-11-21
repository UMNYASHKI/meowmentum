import { createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@/lib/services/helpers/baseAuthQuery';
import {
  LoginRequest,
  LoginResponse,
  OtpValidationRequest,
  PasswordResetRequest,
  PasswordUpdateRequest,
  RegisterRequest, ResetPasswordResponse,
  SendOtpRequest,
  VerificationCodeRequest,
} from '@/lib/services/auth/authDtos';

const controllerRoute: string = 'auth';

export const authApi = createApi({
  reducerPath: 'apiAuth',
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    register: builder.mutation<void, RegisterRequest>({
      query: (credentials) => ({
        url: `/${controllerRoute}/register`,
        method: 'POST',
        body: credentials,
      }),
    }),
    verifyOtp: builder.mutation<void, VerificationCodeRequest>({
      query: (credentials) => ({
        url: `/${controllerRoute}/verify-otp`,
        method: 'POST',
        body: credentials,
      }),
    }),
    login: builder.mutation<LoginResponse, LoginRequest>({
      query: (credentials) => ({
        url: `/${controllerRoute}/login`,
        method: 'POST',
        body: credentials,
      }),
    }),
    sendOtp: builder.mutation<void, PasswordResetRequest>({
      query: (credentials) => ({
        url: `/${controllerRoute}/send-reset-otp`,
        method: 'POST',
        body: credentials,
      }),
    }),
    verifyResetOtp: builder.mutation<
      ResetPasswordResponse,
      OtpValidationRequest
    >({
      query: (credentials) => ({
        url: `/${controllerRoute}/verify-reset-otp`,
        method: 'POST',
        body: credentials,
      }),
    }),
    resetPassword: builder.mutation<void, PasswordUpdateRequest>({
      query: (credentials) => ({
        url: `/${controllerRoute}/reset-password`,
        method: 'POST',
        body: credentials,
      }),
    }),
    logOut: builder.mutation<void, {}>({
      query: (credentials) => ({
        url: `/${controllerRoute}/logout`,
        method: 'POST',
        body: credentials,
      }),
    }),
  }),
});

export const {
  useRegisterMutation,
  useVerifyOtpMutation,
  useLoginMutation,
  useSendOtpMutation,
  useVerifyResetOtpMutation,
  useResetPasswordMutation,
  useLogOutMutation,
} = authApi;
