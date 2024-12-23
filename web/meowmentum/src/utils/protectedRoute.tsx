'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect } from 'react';
import { useAuth } from '@providers/authProvider';
import { Spinner } from '@nextui-org/react';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  useEffect(() => {
    if (!isAuthenticated && !isLoading) {
      router.push('/login');
    }
  }, [isAuthenticated, isLoading, router]);

  if (!isAuthenticated) {
    return (
      <Spinner
        size="lg"
        className={'flex items-center justify-center h-screen'}
      />
    );
  }

  return <>{children}</>;
};

export default ProtectedRoute;
