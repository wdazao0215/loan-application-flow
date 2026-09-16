import type { ReactNode } from "react";

export type ControlProps = {
  id: string;
  name: string;
  "aria-invalid": boolean;
  "aria-describedby"?: string;
};

type FormFieldProps = {
  name: string;
  label: string;
  hint?: string;
  error?: string;
  className?: string;
  children: (controlProps: ControlProps) => ReactNode;
};

export function controlClassName(hasError: boolean) {
  return [
    "block w-full rounded-lg border bg-white px-3.5 py-2.5 text-base text-ink shadow-xs outline-none transition",
    "placeholder:text-ink-subtle focus:ring-4",
    hasError
      ? "border-danger-500 focus:border-danger-500 focus:ring-danger-100"
      : "border-line focus:border-brand-600 focus:ring-brand-100",
  ].join(" ");
}

export function FormField({ name, label, hint, error, className, children }: FormFieldProps) {
  const hintId = hint ? `${name}-hint` : undefined;
  const errorId = error ? `${name}-error` : undefined;
  const describedBy = [errorId, hintId].filter(Boolean).join(" ") || undefined;

  return (
    <div className={className}>
      <label htmlFor={name} className="mb-1.5 block text-sm font-medium text-ink">
        {label}
      </label>
      {children({ id: name, name, "aria-invalid": Boolean(error), "aria-describedby": describedBy })}
      {error ? (
        <p id={errorId} className="mt-1.5 text-sm text-danger-600">
          {error}
        </p>
      ) : null}
      {hint ? (
        <p id={hintId} className="mt-1.5 text-sm text-ink-muted">
          {hint}
        </p>
      ) : null}
    </div>
  );
}
