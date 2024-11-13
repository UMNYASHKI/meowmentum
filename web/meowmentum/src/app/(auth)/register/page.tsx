'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import {
  useRegisterMutation,
  useVerifyOtpMutation,
} from '@services/auth/authApi';
import TextInput from '@common/textinput';
import Button from '@common/button';
import GoogleIcon from '@public/google.svg';
import { useAppDispatch } from '@/lib/hooks';
import { setPopupMessage } from '@/lib/slices/app/appSlice';

export default function Register() {
  const dispatch = useAppDispatch();
  const [registerPost] = useRegisterMutation();
  const [verifyOtp] = useVerifyOtpMutation();
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: '',
    name: '',
    password: '',
    verificationCode: '',
  });
  const [isVerificationRequired, setIsVerificationRequired] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleChange = (e: any) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: any) => {
    e.preventDefault();

    if (isVerificationRequired) {
      await verifyCode();
      return;
    }

    if (formData.password.trim().length < 8) {
      dispatch(
        setPopupMessage({
          message: 'Password is too small',
          type: 'error',
          isVisible: true,
        })
      );
      return;
    }

    try {
      await registerPost({
        email: formData.email,
        username: formData.name,
        password: formData.password,
      }).unwrap();

      setIsVerificationRequired(true);
    } catch (err) {
      // TODO Check the error password failure
      dispatch(
        setPopupMessage({
          message: 'An error occurred during registration',
          type: 'error',
          isVisible: true,
        })
      );
      return;
    }
  };

  const verifyCode = async () => {
    try {
      await verifyOtp({
        email: formData.email,
        otpCode: formData.verificationCode,
      }).unwrap();

      router.push('/login');
    } catch (err) {
      dispatch(
        setPopupMessage({
          message: 'Invalid code',
          type: 'error',
          isVisible: true,
        })
      );
      return;
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6 max-w-sm mx-auto">
      <h2 className="text-3xl font-bold text-primary">Create an account</h2>

      <div className="space-y-4">
        <TextInput
          label="Name"
          type="text"
          name="name"
          placeholder="Name"
          value={formData.name}
          onChange={handleChange}
          disabled={isVerificationRequired}
          className={'border-primary'}
        />

        <TextInput
          label="Email"
          type="email"
          name="email"
          placeholder="Email"
          value={formData.email}
          onChange={handleChange}
          disabled={isVerificationRequired}
          className={'border-primary'}
        />

        <TextInput
          label="Password"
          type="password"
          name="password"
          placeholder="Password"
          value={formData.password}
          onChange={handleChange}
          disabled={isVerificationRequired}
          className={'border-primary'}
        />
        {!isVerificationRequired ? (
          <p className="text-sm text-gray-500">
            Must be at least 8 characters.
          </p>
        ) : (
          ''
        )}

        {isVerificationRequired && (
          <TextInput
            label="Verification Code"
            type="text"
            name="verificationCode"
            placeholder="Verification Code"
            value={formData.verificationCode}
            onChange={handleChange}
            className={'border-primary'}
          />
        )}
      </div>

      <div className="space-y-4">
        <Button
          type="submit"
          className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
        >
          {isVerificationRequired ? 'Verify' : 'Sign Up'}
        </Button>

        {!isVerificationRequired ? (
          <Button
            bordered
            borderColor="border-gray-900"
            className="w-full py-3 rounded-full hover:bg-gray-100"
            icon={
              <GoogleIcon alt="Google Icon" className="w-5 h-5 text-primary" />
            }
          >
            Continue with Google
          </Button>
        ) : (
          ''
        )}
      </div>

      {/* Error Message */}
      {errorMessage && (
        <h3 className="text-red-500 text-center mt-4">{errorMessage}</h3>
      )}
      {!isVerificationRequired ? (
        <p className="text-center text-gray-500 mt-4">
          Already have an account?{' '}
          <a href="/login" className="text-gray-900 hover:underline">
            Log in
          </a>
        </p>
      ) : (
        ''
      )}
    </form>
  );
}
