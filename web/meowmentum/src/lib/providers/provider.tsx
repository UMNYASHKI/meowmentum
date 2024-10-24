'use client';

import { Provider } from 'react-redux';
import { store } from '../store';
import { AuthProvider } from '@/lib/providers/authProvider';

export function ProvidersComponent({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <Provider store={store}>
      <AuthProvider>{children}</AuthProvider>
    </Provider>
  );
}
