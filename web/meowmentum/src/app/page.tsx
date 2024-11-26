'use client';
import ProtectedRoute from '@utils/protectedRoute';
import Tasks from '@/app/(main)/tasks/page';
import MainLayout from '@/app/(main)/layout';

export default function Home() {
  return (
    <ProtectedRoute>
      <MainLayout>
        <Tasks />
      </MainLayout>
    </ProtectedRoute>
  );
}
