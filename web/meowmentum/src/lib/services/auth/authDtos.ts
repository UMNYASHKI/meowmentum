export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface VerificationCodeRequest {
  email: string;
  otpCode: string;
}

export interface SendOtpRequest {
  email: string;
}

export interface PasswordResetRequest {
  email: string;
}

export interface OtpValidationRequest {
  email: string;
  otpCode: string;
}

export interface PasswordUpdateRequest {
  resetToken: string;
  email: string;
  newPassword: string;
}

export interface ResetPasswordResponse {
  resetToken: string;
}
