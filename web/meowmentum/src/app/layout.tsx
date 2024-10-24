import localFont from 'next/font/local';
import '@styles/globals.css';
import { ProvidersComponent } from '@/lib/providers/provider';

// const geistSans = localFont({
//   src: 'src/styles/fonts/GeistVF.woff',
//   variable: '--font-geist-sans',
//   weight: '100 900',
// });
// const geistMono = localFont({
//   src: 'src/styles/fonts/GeistMonoVF.woff',
//   variable: '--font-geist-mono',
//   weight: '100 900',
// });

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <ProvidersComponent>{children}</ProvidersComponent>
      </body>
    </html>
  );
}
