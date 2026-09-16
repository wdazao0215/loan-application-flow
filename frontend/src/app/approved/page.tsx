import type { Metadata } from "next";
import Link from "next/link";

export const metadata: Metadata = { title: "Application approved" };

export default async function ApprovedPage({ searchParams }: PageProps<"/approved">) {
  const { reference, returning } = await searchParams;
  const isReturningCustomer = returning === "true";

  return (
    <article className="mx-auto max-w-xl rounded-2xl border border-line bg-white p-6 text-center shadow-sm sm:p-10">
      <span aria-hidden="true" className="mx-auto grid size-14 place-items-center rounded-full bg-brand-100 text-brand-700">
        <svg viewBox="0 0 24 24" className="size-7" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
          <path d="m5 12.5 4.5 4.5L19 7.5" />
        </svg>
      </span>

      <h1 className="mt-6 text-2xl font-semibold tracking-tight text-ink sm:text-3xl">
        {isReturningCustomer ? "Your application is approved and updated" : "Your application is approved"}
      </h1>
      <p className="mt-3 text-base text-ink-muted">
        {isReturningCustomer
          ? "We found the application you already had with us and updated it with the details you just sent. You still have a single application."
          : "Congratulations! We're sharing your application with our loan servicing partner, so there's nothing else you need to do right now."}
      </p>

      {typeof reference === "string" ? (
        <div className="mt-8 rounded-xl bg-canvas px-4 py-3">
          <p className="text-xs font-medium uppercase tracking-wide text-ink-muted">Application reference</p>
          <p className="mt-1 break-all font-mono text-sm text-ink">{reference}</p>
        </div>
      ) : null}

      <p className="mt-8 text-sm text-ink-muted">
        Need to change something? Submit the form again with the same Social Security number.
      </p>
      <Link
        href="/"
        className="mt-4 inline-flex items-center justify-center rounded-lg border border-line px-5 py-2.5 text-sm font-semibold text-ink transition hover:bg-canvas focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-600"
      >
        Back to the application form
      </Link>
    </article>
  );
}
