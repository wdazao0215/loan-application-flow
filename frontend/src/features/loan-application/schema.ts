import { z } from "zod";
import { US_STATES } from "./us-states";

const MAX_REQUESTED_AMOUNT = 1_000_000_000;

const requiredText = (message: string, maxLength: number) =>
  z.string().trim().min(1, message).max(maxLength, `Use ${maxLength} characters or fewer`);

export const loanApplicationSchema = z.object({
  firstName: requiredText("Enter your first name", 100),
  lastName: requiredText("Enter your last name", 100),
  companyName: requiredText("Enter your company name", 150),
  street: requiredText("Enter your street address", 200),
  city: requiredText("Enter your city", 100),
  state: z.enum(
    US_STATES.map((state) => state.code),
    { error: "Select your state" },
  ),
  zipCode: z
    .string()
    .trim()
    .regex(/^\d{5}(-\d{4})?$/, "Enter a 5-digit ZIP code"),
  requestedAmount: z
    .string()
    .transform((amount) => amount.replace(/[$,\s]/g, ""))
    .pipe(
      z
        .string()
        .min(1, "Enter the amount you want to borrow")
        .regex(/^\d+(\.\d{1,2})?$/, "Enter an amount in dollars, like 25,000")
        .transform(Number)
        .refine((amount) => amount > 0, "Enter an amount greater than $0")
        .refine((amount) => amount <= MAX_REQUESTED_AMOUNT, "Enter an amount up to $1,000,000,000"),
    ),
  ssn: z
    .string()
    .trim()
    .regex(/^\d{3}-?\d{2}-?\d{4}$/, "Enter your 9-digit Social Security number"),
});

export type LoanApplicationField = keyof typeof loanApplicationSchema.shape;

export type LoanApplicationInput = Record<LoanApplicationField, string>;

export type LoanApplication = z.output<typeof loanApplicationSchema>;

export type FieldErrors = Partial<Record<LoanApplicationField, string>>;

export const LOAN_APPLICATION_FIELDS = Object.keys(loanApplicationSchema.shape) as LoanApplicationField[];

export const EMPTY_LOAN_APPLICATION: LoanApplicationInput = {
  firstName: "",
  lastName: "",
  companyName: "",
  street: "",
  city: "",
  state: "",
  zipCode: "",
  requestedAmount: "",
  ssn: "",
};

export function validateField(field: LoanApplicationField, value: string): string | undefined {
  const result = loanApplicationSchema.shape[field].safeParse(value);
  return result.success ? undefined : result.error.issues[0]?.message;
}

export function validateLoanApplication(input: LoanApplicationInput): FieldErrors {
  const result = loanApplicationSchema.safeParse(input);
  if (result.success) {
    return {};
  }

  const fieldErrors = z.flattenError(result.error).fieldErrors as Partial<Record<LoanApplicationField, string[]>>;
  return Object.fromEntries(
    Object.entries(fieldErrors).map(([field, messages]) => [field, messages?.[0]]),
  ) as FieldErrors;
}
