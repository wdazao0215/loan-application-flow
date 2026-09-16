import type { Metadata } from "next";
import { Inter } from "next/font/google";
import Link from "next/link";
import "./globals.css";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: { default: "Apply for a business loan · LoanFlow", template: "%s · LoanFlow" },
  description: "Apply for a business loan and get a decision in seconds.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en" className={inter.variable}>
      <body className="min-h-dvh font-sans antialiased">
        <header className="border-b border-line bg-white">
          <div className="mx-auto flex h-16 max-w-5xl items-center justify-between px-4 sm:px-6">
            <Link href="/" className="flex items-center gap-2.5 font-semibold text-ink">
              <span aria-hidden="true" className="grid size-8 place-items-center rounded-lg bg-brand-700 text-sm font-bold text-white">
                LF
              </span>
              LoanFlow
            </Link>
            <span className="text-sm text-ink-muted">Business loans</span>
          </div>
        </header>
        <main className="mx-auto max-w-5xl px-4 py-10 sm:px-6 sm:py-14">{children}</main>
      </body>
    </html>
  );
}
