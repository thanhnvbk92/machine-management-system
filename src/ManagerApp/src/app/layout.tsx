import type { Metadata } from 'next';
import './globals.css';
import { AuthProvider } from '@/contexts/AuthContextSimple';
import AuthGuard from '@/components/AuthGuardSimple';

export const metadata: Metadata = {
  title: 'Machine Management System',
  description: 'Machine Management System for Industrial Operations',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="vi">
      <body className="font-sans antialiased">
        <AuthProvider>
          <AuthGuard>
            {children}
          </AuthGuard>
        </AuthProvider>
      </body>
    </html>
  );
}