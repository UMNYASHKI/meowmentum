'use client';

import { useAppDispatch, useAppSelector } from '@/lib/hooks';
import {
  hidePopupMessage,
  selectPopupMessage,
} from '@/lib/slices/app/appSlice';
import { twMerge } from 'tailwind-merge';

export default function PopupMessage() {
  const dispatch = useAppDispatch();
  const { message, isVisible, type } = useAppSelector(selectPopupMessage);

  if (!isVisible) return null;

  const baseClasses =
    'fixed bottom-4 left-4 p-4 rounded-lg shadow-lg max-w-xs transition-opacity duration-300 z-[1050]';
  const typeClasses =
    type === 'error'
      ? 'bg-red-100 text-red-800 border border-red-500'
      : type === 'success'
        ? 'bg-green-100 text-green-800 border border-green-500'
        : 'bg-gray-100 text-gray-800';

  return (
    <div className={twMerge(baseClasses, typeClasses)}>
      <div className="flex justify-between items-center">
        <span>{message}</span>
        <button
          onClick={() => dispatch(hidePopupMessage())}
          className="ml-4 text-gray-500 hover:text-gray-800"
        >
          &times;
        </button>
      </div>
    </div>
  );
}
