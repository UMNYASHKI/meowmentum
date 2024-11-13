'use client';

import TextInput from '@common/textinput';
import Button from '@common/button';
import GoogleIcon from '@public/google.svg';
import React, { FormEvent, useState } from 'react';
import { useLoginMutation } from '@services/auth/authApi';
import { useRouter } from 'next/navigation';
import { useAppDispatch } from '@/lib/hooks';
import { setPopupMessage } from '@/lib/slices/app/appSlice';
import { useAuth } from '@providers/authProvider';

export default function LogIn() {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const { login } = useAuth();
  const [serverLogin] = useLoginMutation();
  const router = useRouter();
  const dispatch = useAppDispatch();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    try {
      await serverLogin({
        email: formData.email,
        password: formData.password,
      })
        .unwrap()
        .then((data) => login(data.token));

      router.push('/tasks');
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  return (
    <>
      <form onSubmit={handleSubmit} className="space-y-6 max-w-sm mx-auto">
        <h2 className="text-3xl font-bold text-primary">Log In</h2>

        <div className="space-y-4">
          <TextInput
            label="Email"
            type="text"
            name="email"
            placeholder="Email"
            value={formData.email}
            onChange={handleChange}
            className={'border-primary'}
          />

          <TextInput
            label="Password"
            type="password"
            name="password"
            placeholder="Password"
            value={formData.password}
            onChange={handleChange}
            className={'border-primary'}
          />

          <a
            className="block text-right text-gray-900 hover:underline"
            href="/forgot"
          >
            Forgot password?
          </a>
        </div>

        <div className="space-y-4">
          <Button
            type="submit"
            className="w-full py-3 rounded-full bg-primary hover:bg-button-hover"
          >
            Log In
          </Button>

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
        </div>

        <p className="text-center text-gray-500 mt-4">
          Don&#39;t have an account?{' '}
          <a href="/register" className="text-gray-900 hover:underline">
            Sign Up
          </a>
        </p>
      </form>
    </>
  );
}
