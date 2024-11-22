import '@styles/globals.css';
import { ProvidersComponent } from '@/lib/providers/provider';
import PopupMessage from '@common/popupMessage';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  //  className="dark"
  return (
    <html lang="en" className="dark">
      <body>
        <ProvidersComponent>
          {children}
          <footer>
            <PopupMessage></PopupMessage>
          </footer>
        </ProvidersComponent>
      </body>
    </html>
  );
}
