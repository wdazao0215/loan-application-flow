import type { Metadata } from "next";
import Link from "next/link";
import { describeDenialReasons } from "@/features/loan-application/denial-reasons";

export const metadata: Metadata = { title: "Application not approved" };

export default async function DeniedPage({ searchParams }: PageProps<"/denied">) {
  const { reason } = await searchParams;
  const reasons = describeDenialReasons(reason === undefined ? [] : [reason].flat());

  return (
    <article className="mx-auto max-w-xl rounded-2xl border border-line bg-white p-6 shadow-sm sm:p-10">
      <span aria-hidden="true" className="grid size-14 place-items-center rounded-full bg-caution-50 text-caution-600">
        <svg viewBox="0 0 24 24" className="size-7" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round">
          <path d="M12 7.5v5.5M12 16.5h.01" />
        </svg>
      </span>

      <h1 className="mt-6 text-2xl font-semibold tracking-tight text-ink sm:text-3xl">
        We can&apos;t approve your application
      </h1>
      <p className="mt-3 text-base text-ink-muted">
        We&apos;re sorry we couldn&apos;t approve your loan this time. None of the details you entered were saved.
      </p>

      <ul className="mt-8 space-y-4">
        {reasons.map((denialReason) => (
          <li key={denialReason.title} className="rounded-xl border border-line p-4">
            <h2 className="text-base font-semibold text-ink">{denialReason.title}</h2>
            <p className="mt-1 text-sm text-ink-muted">{denialReason.explanation}</p>
            <p className="mt-2 text-sm text-ink">{denialReason.nextStep}</p>
          </li>
        ))}
      </ul>

      <Link
        href="/"
        className="mt-8 inline-flex w-full items-center justify-center rounded-lg bg-brand-700 px-5 py-3 text-sm font-semibold text-white transition hover:bg-brand-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-600 sm:w-auto"
      >
        Start a new application
      </Link>
    </article>
  );
}
