'use client';

import TextInput from '@common/textinput';
import Button from '@common/button';
import GoogleIcon from '@public/google.svg';
import React, { FormEvent, SyntheticEvent, useState } from 'react';
import {
  useLoginMutation,
  useSendOtpMutation,
  useVerifyOtpMutation,
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

  const [sendOtp] = useSendOtpMutation();
  const [verifyOtp] = useVerifyOtpMutation();
  const router = useRouter();
  const dispatch = useAppDispatch();

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
          message: 'Invalid email or password',
          type: 'error',
          isVisible: true,
        })
      );
    }
  };

  return (
    <>
      {!isOtpSent ? (
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

          <div className="space-y-4">
            <button
              onClick={handleSendOtp}
              className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
            >
              Send Instructions
            </button>
          </div>
        </div>
      ) : (
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
              label="Password"
              type="password"
              name="password"
              placeholder="New Password"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={'border-primary'}
            />
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

          <div className="space-y-4">
            <button
              onClick={handleSendOtp}
              className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
            >
              Send Instructions
            </button>
          </div>
        </div>
      )}
    </>
  );
}
