"use server";

import { redirect } from "next/navigation";
import { sendLoanApplication } from "./loan-applications-api";
import { loanApplicationSchema, type LoanApplicationInput } from "./schema";

export type SubmissionFailure = { message: string };

export async function submitLoanApplication(input: LoanApplicationInput): Promise<SubmissionFailure> {
  const parsed = loanApplicationSchema.safeParse(input);
  if (!parsed.success) {
    return { message: "Some answers need your attention. Review the highlighted fields and try again." };
  }

  const outcome = await sendLoanApplication(parsed.data);

  switch (outcome.kind) {
    case "approved":
      redirect(`/approved?${new URLSearchParams({
        reference: outcome.applicationId,
        returning: String(outcome.isReturningCustomer),
      })}`);
    case "denied":
      redirect(`/denied?${new URLSearchParams(outcome.reasonCodes.map((code) => ["reason", code]))}`);
    case "rejected":
      return { message: outcome.message };
    case "unavailable":
      return { message: "We couldn't reach our servers. Your answers are still here, so please try again in a moment." };
  }
}
