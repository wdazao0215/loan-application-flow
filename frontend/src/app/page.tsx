import { LoanApplicationForm } from "@/features/loan-application/components/LoanApplicationForm";

const HIGHLIGHTS = [
  { title: "A decision in seconds", body: "You'll know right away whether your application is approved." },
  { title: "One application per person", body: "Applying again with the same SSN updates the application you already have." },
  { title: "Nothing kept if you're not approved", body: "We only save your details when your application is approved." },
];

export default function ApplyPage() {
  return (
    <div className="grid gap-10 lg:grid-cols-[minmax(0,1fr)_minmax(0,1.6fr)] lg:gap-14">
      <section className="lg:pt-2">
        <p className="text-sm font-semibold uppercase tracking-wide text-brand-700">Business loan application</p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight text-ink sm:text-4xl">
          Apply for a business loan
        </h1>
        <p className="mt-4 text-base text-ink-muted">
          Tell us about you and your business. It takes about two minutes.
        </p>
        <ul className="mt-8 space-y-5">
          {HIGHLIGHTS.map((highlight) => (
            <li key={highlight.title} className="flex gap-3">
              <span aria-hidden="true" className="mt-1 grid size-5 shrink-0 place-items-center rounded-full bg-brand-100 text-brand-700">
                <svg viewBox="0 0 20 20" className="size-3.5" fill="currentColor">
                  <path d="M8.2 13.6 4.6 10l-1.2 1.2 4.8 4.8L17 7.2 15.8 6z" />
                </svg>
              </span>
              <span>
                <span className="block text-sm font-semibold text-ink">{highlight.title}</span>
                <span className="block text-sm text-ink-muted">{highlight.body}</span>
              </span>
            </li>
          ))}
        </ul>
      </section>

      <section aria-label="Application form" className="rounded-2xl border border-line bg-white p-5 shadow-sm sm:p-8">
        <LoanApplicationForm />
      </section>
    </div>
  );
}
