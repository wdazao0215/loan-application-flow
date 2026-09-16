import type { LoanApplication } from "./schema";

const API_URL = process.env.API_URL ?? "http://localhost:8080";
const REQUEST_TIMEOUT_MS = 10_000;

type ApiDecision =
  | { decision: "approved"; applicationId: string; isReturningCustomer: boolean }
  | { decision: "denied"; reasons: { code: string; message: string }[] };

type ApiProblem = { title?: string; detail?: string };

export type SubmissionOutcome =
  | { kind: "approved"; applicationId: string; isReturningCustomer: boolean }
  | { kind: "denied"; reasonCodes: string[] }
  | { kind: "rejected"; message: string }
  | { kind: "unavailable" };

export async function sendLoanApplication(application: LoanApplication): Promise<SubmissionOutcome> {
  let response: Response;

  try {
    response = await fetch(`${API_URL}/api/loan-applications`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(toApiRequest(application)),
      cache: "no-store",
      signal: AbortSignal.timeout(REQUEST_TIMEOUT_MS),
    });
  } catch {
    return { kind: "unavailable" };
  }

  if (response.ok) {
    return toOutcome((await response.json()) as ApiDecision);
  }

  if (response.status === 400 || response.status === 409) {
    const problem = (await response.json()) as ApiProblem;
    return { kind: "rejected", message: problem.detail ?? problem.title ?? "Please review your application." };
  }

  return { kind: "unavailable" };
}

function toApiRequest(application: LoanApplication) {
  return {
    firstName: application.firstName,
    lastName: application.lastName,
    companyName: application.companyName,
    address: {
      street: application.street,
      city: application.city,
      state: application.state,
      zipCode: application.zipCode,
    },
    ssn: application.ssn,
    requestedAmount: application.requestedAmount,
  };
}

function toOutcome(decision: ApiDecision): SubmissionOutcome {
  return decision.decision === "approved"
    ? { kind: "approved", applicationId: decision.applicationId, isReturningCustomer: decision.isReturningCustomer }
    : { kind: "denied", reasonCodes: decision.reasons.map((reason) => reason.code) };
}
