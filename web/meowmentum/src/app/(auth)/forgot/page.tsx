'use client';

import TextInput from '@common/textinput';
import Button from '@common/button';
import GoogleIcon from '@public/google.svg';
import React, { FormEvent, SyntheticEvent, useState } from 'react';
import {
  useLoginMutation,
  useLogOutMutation,
  useResetPasswordMutation,
  useSendOtpMutation,
  useVerifyOtpMutation,
  useVerifyResetOtpMutation,
} from '@services/auth/authApi';
import { useRouter } from 'next/navigation';
import { useAppDispatch } from '@/lib/hooks';
import { setPopupMessage } from '@/lib/slices/app/appSlice';
import { useAuth } from '@providers/authProvider';

export default function Forgot() {
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isOtpSent, setIsOtpSent] = useState<boolean>(false);
  const [isOtpConfirmed, setIsOtpConfirmed] = useState<boolean>(false);
  const [resetToken, setResetToken] = useState<string>('');

  const [sendOtp] = useSendOtpMutation();
  const [verifyOtp] = useVerifyResetOtpMutation();
  const [resetPassword] = useResetPasswordMutation();
  const [logout] = useLogOutMutation();
  const router = useRouter();
  const dispatch = useAppDispatch();
  const auth = useAuth();

  const handleSendOtp = async (e: SyntheticEvent) => {
    e.preventDefault();

    try {
      await sendOtp({
        email: email,
      }).unwrap();

      setIsOtpSent(true);
    } catch (error) {
      dispatch(
        setPopupMessage({
          message: 'Failed to send instructions',
          type: 'error',
          isVisible: true,
        })
      );
    }
  };

  const handleVerifyOtp = async (e: SyntheticEvent) => {
    e.preventDefault();

    try {
      await verifyOtp({
        email: email,
        otpCode: otp,
      })
        .unwrap()
        .then((data) => setResetToken(data.resetToken));

      setIsOtpConfirmed(true);
    } catch (error) {
      dispatch(
        setPopupMessage({
          message: 'Verification code is invalid',
          type: 'error',
          isVisible: true,
        })
      );
    }
  };

  const handleResetPassword = async (e: SyntheticEvent) => {
    e.preventDefault();

    if (newPassword !== confirmPassword) {
      dispatch(
        setPopupMessage({
          message: "Passwords don't match",
          type: 'error',
          isVisible: true,
        })
      );

      return;
    }

    try {
      await resetPassword({
        resetToken: resetToken,
        email: email,
        newPassword: newPassword,
      })
        .unwrap()
        .then(async () => {
          if (auth.isAuthenticated) {
            await logout({}).unwrap();
            auth.logout();
          }
        });

      router.push('/login');
    } catch (error) {
      dispatch(
        setPopupMessage({
          message: 'Failed to change password',
          type: 'error',
          isVisible: true,
        })
      );
    }
  };

  return (
    <>
      {!isOtpConfirmed ? (
        <div>
          <h2 className="text-3xl font-bold text-primary">Forgot password?</h2>
          <br />
          <p className="text-gray-500 text-16">
            Enter the email associated with your account and we’ll send you an
            email with instructions to reset your password.
          </p>
          <br />

          <div className="space-y-4">
            <TextInput
              label="Email"
              type="text"
              name="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={'border-primary'}
            />
          </div>
          <br />

          {isOtpSent ? (
            <>
              <div className="space-y-4">
                <TextInput
                  label="Verification code"
                  type="text"
                  name="otp"
                  placeholder="Verification code"
                  value={otp}
                  onChange={(e) => setOtp(e.target.value)}
                  className={'border-primary'}
                />
              </div>
              <br />

              <div className="space-y-4">
                <button
                  onClick={handleVerifyOtp}
                  className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
                >
                  Verify
                </button>
              </div>
            </>
          ) : (
            <>
              <br />

              <div className="space-y-4">
                <button
                  onClick={handleSendOtp}
                  className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
                >
                  Send Instructions
                </button>
              </div>
            </>
          )}
        </div>
      ) : (
        <>
          <div>
            <h2 className="text-3xl font-bold text-primary">
              Create new password
            </h2>
            <br />
            <p className="text-gray-500 text-16">
              This password should be different from the previous password.
            </p>
            <br />

            <div className="space-y-4">
              <TextInput
                label="New password"
                type="password"
                name="newPassword"
                placeholder="New password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className={'border-primary'}
              />
              <TextInput
                label="Confirm password"
                type="password"
                name="confirmPassword"
                placeholder="Confirm password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className={'border-primary'}
              />
              <p className="text-gray-500 text-16">
                Both passwords should match.
              </p>
            </div>

            <div className="space-y-4">
              <button
                onClick={handleResetPassword}
                className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
              >
                Reset Password
              </button>
            </div>
          </div>
        </>
      )}
    </>
  );
}
