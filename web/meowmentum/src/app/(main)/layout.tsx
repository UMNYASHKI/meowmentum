import ProtectedRoute from '@utils/protectedRoute';
import Section from '@/app/(main)/section';

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    // <div className="flex flex-row h-[100vh]">
    //   <Section />
    //   {children}
    // </div>
    <ProtectedRoute>
      <div className="flex flex-row h-[100vh]">
        <Section />
        {children}
      </div>
    </ProtectedRoute>
  );
}
