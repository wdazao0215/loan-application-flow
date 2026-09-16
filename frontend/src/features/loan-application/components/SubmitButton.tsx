type SubmitButtonProps = {
  isSubmitting: boolean;
};

export function SubmitButton({ isSubmitting }: SubmitButtonProps) {
  return (
    <button
      type="submit"
      disabled={isSubmitting}
      className="inline-flex w-full items-center justify-center gap-2 rounded-lg bg-brand-700 px-5 py-3 text-base font-semibold text-white shadow-sm transition hover:bg-brand-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-600 disabled:cursor-wait disabled:opacity-80 sm:w-auto"
    >
      {isSubmitting ? (
        <>
          <svg aria-hidden="true" viewBox="0 0 24 24" className="size-5 animate-spin">
            <circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" strokeOpacity="0.3" strokeWidth="3" />
            <path d="M21 12a9 9 0 0 0-9-9" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" />
          </svg>
          Reviewing your application…
        </>
      ) : (
        "Submit application"
      )}
    </button>
  );
}
