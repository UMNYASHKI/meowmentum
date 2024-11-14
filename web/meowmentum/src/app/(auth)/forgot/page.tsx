'use client';

import TextInput from '@common/textinput';
import Button from '@common/button';
import GoogleIcon from '@public/google.svg';
import React, { FormEvent, useState } from 'react';
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

  const [sendOtp] = useSendOtpMutation();
  const [verifyOtp] = useVerifyOtpMutation();
  const router = useRouter();
  const dispatch = useAppDispatch();
  const [isOtpSent, setIsOtpSent] = useState<boolean>(false);

  const handleSendOtp = async (e: FormEvent) => {
    e.preventDefault();

    try {
      await sendOtp({
        email: email,
      }).unwrap();

      router.push('/tasks');
    } catch (error) {
      // dispatch(
      //   setPopupMessage({
      //     message: 'Invalid email or password',
      //     type: 'error',
      //     isVisible: true,
      //   })
      // );
    }
  };

  return (
    <>
      {isOtpSent ? (
        <div>
          <h2 className="text-3xl font-bold text-primary">Forgot password?</h2>
          <p className="text-gray-900">
            Enter the email associated with your account and we’ll send you an
            email with instructions to reset your password.
          </p>

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

          <div className="space-y-4">
            <Button
              type="submit"
              className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
            >
              Log In
            </Button>
          </div>

          <p className="text-center text-gray-500 mt-4">Send Instructions</p>
        </div>
      ) : (
        ''
      )}
    </>
  );
}
