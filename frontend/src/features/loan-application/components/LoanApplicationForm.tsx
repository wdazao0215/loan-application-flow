"use client";

import { useRef, useState, useTransition, type FormEvent } from "react";
import { formatAmount, sanitizeAmount } from "../formatting";
import {
  EMPTY_LOAN_APPLICATION,
  LOAN_APPLICATION_FIELDS,
  validateField,
  validateLoanApplication,
  type FieldErrors,
  type LoanApplicationField,
  type LoanApplicationInput,
} from "../schema";
import { submitLoanApplication } from "../submit-loan-application";
import { US_STATES } from "../us-states";
import { controlClassName, FormField } from "./FormField";
import { FormSection } from "./FormSection";
import { SsnInput } from "./SsnInput";
import { SubmitButton } from "./SubmitButton";

export function LoanApplicationForm() {
  const [values, setValues] = useState<LoanApplicationInput>(EMPTY_LOAN_APPLICATION);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [submissionError, setSubmissionError] = useState<string>();
  const [isSubmitting, startSubmitting] = useTransition();
  const formRef = useRef<HTMLFormElement>(null);

  function change(field: LoanApplicationField, value: string) {
    setValues((current) => ({ ...current, [field]: value }));
    if (errors[field]) {
      setErrors((current) => ({ ...current, [field]: validateField(field, value) }));
    }
  }

  function validateOnBlur(field: LoanApplicationField, value = values[field]) {
    if (value !== "") {
      setErrors((current) => ({ ...current, [field]: validateField(field, value) }));
    }
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSubmissionError(undefined);

    const fieldErrors = validateLoanApplication(values);
    setErrors(fieldErrors);

    const firstInvalidField = LOAN_APPLICATION_FIELDS.find((field) => fieldErrors[field]);
    if (firstInvalidField) {
      formRef.current?.querySelector<HTMLElement>(`[name="${firstInvalidField}"]`)?.focus();
      return;
    }

    startSubmitting(async () => {
      const failure = await submitLoanApplication(values);
      if (failure) {
        setSubmissionError(failure.message);
      }
    });
  }

  const textInput = (field: LoanApplicationField, autoComplete: string) => ({
    value: values[field],
    autoComplete,
    onChange: (event: { target: { value: string } }) => change(field, event.target.value),
    onBlur: () => validateOnBlur(field),
    className: controlClassName(Boolean(errors[field])),
  });

  return (
    <form ref={formRef} onSubmit={handleSubmit} noValidate className="space-y-8">
      <FormSection title="About you">
        <FormField name="firstName" label="First name" error={errors.firstName} className="sm:col-span-3">
          {(control) => <input {...control} type="text" {...textInput("firstName", "given-name")} />}
        </FormField>
        <FormField name="lastName" label="Last name" error={errors.lastName} className="sm:col-span-3">
          {(control) => <input {...control} type="text" {...textInput("lastName", "family-name")} />}
        </FormField>
      </FormSection>

      <FormSection title="Your business">
        <FormField name="companyName" label="Company name" error={errors.companyName} className="sm:col-span-6">
          {(control) => <input {...control} type="text" {...textInput("companyName", "organization")} />}
        </FormField>
      </FormSection>

      <FormSection title="Address" description="Where you or your business are based.">
        <FormField name="street" label="Street address" error={errors.street} className="sm:col-span-6">
          {(control) => <input {...control} type="text" {...textInput("street", "address-line1")} />}
        </FormField>
        <FormField name="city" label="City" error={errors.city} className="sm:col-span-6 md:col-span-2">
          {(control) => <input {...control} type="text" {...textInput("city", "address-level2")} />}
        </FormField>
        <FormField name="state" label="State" error={errors.state} className="sm:col-span-3 md:col-span-2">
          {(control) => (
            <select
              {...control}
              value={values.state}
              autoComplete="address-level1"
              onChange={(event) => {
                change("state", event.target.value);
                validateOnBlur("state", event.target.value);
              }}
              onBlur={() => validateOnBlur("state")}
              className={controlClassName(Boolean(errors.state))}
            >
              <option value="" disabled>
                Select a state
              </option>
              {US_STATES.map((state) => (
                <option key={state.code} value={state.code}>
                  {state.name}
                </option>
              ))}
            </select>
          )}
        </FormField>
        <FormField name="zipCode" label="ZIP code" error={errors.zipCode} className="sm:col-span-3 md:col-span-2">
          {(control) => <input {...control} type="text" inputMode="numeric" {...textInput("zipCode", "postal-code")} />}
        </FormField>
      </FormSection>

      <FormSection title="Your loan">
        <FormField
          name="requestedAmount"
          label="Requested amount"
          hint="Whole dollars or dollars and cents."
          error={errors.requestedAmount}
          className="sm:col-span-3"
        >
          {(control) => (
            <div className="relative">
              <span aria-hidden="true" className="pointer-events-none absolute inset-y-0 left-3.5 flex items-center text-ink-muted">
                $
              </span>
              <input
                {...control}
                type="text"
                inputMode="decimal"
                autoComplete="off"
                placeholder="25,000"
                value={values.requestedAmount}
                onChange={(event) => change("requestedAmount", sanitizeAmount(event.target.value))}
                onFocus={() => change("requestedAmount", values.requestedAmount.replace(/,/g, ""))}
                onBlur={() => {
                  const formatted = formatAmount(values.requestedAmount);
                  change("requestedAmount", formatted);
                  validateOnBlur("requestedAmount", formatted);
                }}
                className={`${controlClassName(Boolean(errors.requestedAmount))} pl-7 tabular-nums`}
              />
            </div>
          )}
        </FormField>
      </FormSection>

      <FormSection
        title="Identity verification"
        description="We only use your Social Security number to verify who you are. Submitting again with the same number updates your existing application."
      >
        <FormField name="ssn" label="Social Security number" error={errors.ssn} className="sm:col-span-3">
          {(control) => (
            <SsnInput
              {...control}
              value={values.ssn}
              onChange={(value) => change("ssn", value)}
              onBlur={() => validateOnBlur("ssn")}
            />
          )}
        </FormField>
      </FormSection>

      <div className="space-y-4 border-t border-line pt-6">
        {submissionError ? (
          <p role="alert" className="rounded-lg border border-danger-200 bg-danger-50 px-4 py-3 text-sm text-danger-700">
            {submissionError}
          </p>
        ) : null}
        <SubmitButton isSubmitting={isSubmitting} />
      </div>
    </form>
  );
}
