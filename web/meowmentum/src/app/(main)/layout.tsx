import ProtectedRoute from '@utils/protectedRoute';
import Section from '@/app/(main)/section';
import Dashboard from '@/app/(main)/dashboard';

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ProtectedRoute>
      <div className="flex flex-row">
        <Section />
        <Dashboard>{children}</Dashboard>
      </div>
    </ProtectedRoute>
  );
}
