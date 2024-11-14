import LogoIcon from '../../../public/logo.svg';
import DashboardIcon from '../../../public/dashboard.svg';

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="relative flex min-h-screen bg-background-dark overflow-hidden">
      <div className="w-1/2 text-white p-16 flex flex-col justify-between relative z-10">
        <div className="space-y-8">
          <div className="flex items-center space-x-3">
            <LogoIcon alt="Logo" className="w-8 h-8" />
          </div>

          <div>
            <h2 className="text-4xl font-bold">Welcome to Meowmentum!</h2>
            <p className="mt-4 text-lg text-gray-400 leading-relaxed">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vulputate
              ut laoreet velit ma.
            </p>
          </div>
        </div>
      </div>

      <div className="absolute bottom-0 left-0 w-[70%] translate-x-1/5 translate-y-1/3 z-10">
        <DashboardIcon
          alt="Dashboard preview"
          className="w-full h-full rounded-lg"
        />
      </div>

      <div className="w-1/2 bg-white flex items-center justify-center relative z-20 shadow-2xl">
        <div className="w-full max-w-md p-8">{children}</div>
      </div>
    </div>
  );
}
