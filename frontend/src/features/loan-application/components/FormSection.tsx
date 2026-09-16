import type { ReactNode } from "react";

type FormSectionProps = {
  title: string;
  description?: string;
  children: ReactNode;
};

export function FormSection({ title, description, children }: FormSectionProps) {
  return (
    <fieldset className="border-t border-line pt-6 first:border-t-0 first:pt-0">
      <legend className="float-left w-full">
        <span className="block text-base font-semibold text-ink">{title}</span>
        {description ? <span className="mt-1 block text-sm text-ink-muted">{description}</span> : null}
      </legend>
      <div className="clear-left grid gap-x-4 gap-y-5 pt-4 sm:grid-cols-6">{children}</div>
    </fieldset>
  );
}
