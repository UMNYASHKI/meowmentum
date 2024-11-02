'use client';

import { Provider } from 'react-redux';
import { store } from '../store';
import { AuthProvider } from '@/lib/providers/authProvider';
import { NextUIProvider } from '@nextui-org/react';
import { useRouter } from 'next/navigation';

export function ProvidersComponent({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();

  return (
    <NextUIProvider navigate={router.push}>
      <Provider store={store}>
        <AuthProvider>{children}</AuthProvider>
      </Provider>
    </NextUIProvider>
  );
}
