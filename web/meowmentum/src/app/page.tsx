'use client';
import ProtectedRoute from '@utils/protectedRoute';
import Tasks from '@/app/(main)/tasks/page';

export default function Home() {
  return (
    <ProtectedRoute>
      <Tasks />
    </ProtectedRoute>
  );
}
