import ProtectedRoute from '@utils/protectedRoute';

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ProtectedRoute>
      <div>
        <div>Header</div>
        {children}
        <div>Footer</div>
      </div>
    </ProtectedRoute>
  );
}
