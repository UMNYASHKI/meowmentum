'use client';

import { setPopupMessage } from '@/lib/slices/app/appSlice';
import { useAppDispatch } from '@/lib/hooks';

export function useSetError() {
  const dispatch = useAppDispatch();

  const setError = (message: string) => {
    dispatch(
      setPopupMessage({
        message: message || 'Error happens',
        type: 'error',
        isVisible: true,
      })
    );
  };

  return setError;
}
