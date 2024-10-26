'use client';

import { Button, Input } from '@nextui-org/react';
import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useRegisterMutation } from '@services/auth/authApi';
import Container from '@common/container';

export default function Register() {
  const [registerPost] = useRegisterMutation();
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: '',
    name: '',
    password: '',
    verificationCode: '',
  });
  const [isVerificationRequired, setIsVerificationRequired] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [verificationCode, setVerificationCode] = useState('');

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

    try {
      const result = await registerPost({
        email: formData.email,
        name: formData.name,
        password: formData.password,
      }).unwrap();

      // TODO logic with error message

      setVerificationCode(result.verificationCode);
      setIsVerificationRequired(true);
    } catch (err) {
      setErrorMessage('An error occurred during registration');
    }
  };

  const verifyCode = async () => {
    if (formData.verificationCode == verificationCode) {
      router.push('/login');
    } else {
      setErrorMessage('Invalid code');
    }
  };

  return (
    <Container>
      <form onSubmit={handleSubmit} className="space-y-4">
        <h2 className="text-center text-2xl font-bold mb-6 text-slate-950">
          Register
        </h2>
        <Input
          label="Email"
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          required
          fullWidth
          className="w-full"
        />

        <Input
          label="Name"
          type="text"
          name="name"
          value={formData.name}
          onChange={handleChange}
          required
          fullWidth
          className="w-full"
        />

        <Input
          label="Password"
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          required
          fullWidth
          className="w-full"
        />

        {isVerificationRequired && (
          <Input
            label="Verification Code"
            type="text"
            name="verificationCode"
            value={formData.verificationCode}
            onChange={handleChange}
            required
            fullWidth
            className="w-full"
          />
        )}

        <div className="flex justify-end">
          <Button type="submit" color="primary">
            {isVerificationRequired ? 'Verify Code' : 'Register'}
          </Button>
        </div>

        {errorMessage && (
          <h3 className="text-red-500 text-center mt-4">{errorMessage}</h3>
        )}
      </form>
    </Container>
  );
}
