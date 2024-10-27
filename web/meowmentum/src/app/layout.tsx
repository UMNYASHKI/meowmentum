import '@styles/globals.css';
import { ProvidersComponent } from '@/lib/providers/provider';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  //  className="dark"
  return (
    <html lang="en" className="dark">
      <body>
        <ProvidersComponent>{children}</ProvidersComponent>
      </body>
    </html>
  );
}
