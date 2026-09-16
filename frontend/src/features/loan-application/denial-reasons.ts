type DenialReasonCopy = { title: string; explanation: string; nextStep: string };

const DENIAL_REASONS: Record<string, DenialReasonCopy> = {
  "state-not-served": {
    title: "We don't lend in your state yet",
    explanation: "Our loans aren't available for businesses located in the state you entered.",
    nextStep: "We're working on reaching more states. You're welcome to apply again once we lend where you are.",
  },
  "identity-not-verified": {
    title: "We couldn't verify your identity",
    explanation: "We weren't able to verify the identity information you provided.",
    nextStep: "Check that your Social Security number is correct. If it is, contact our support team.",
  },
};

const UNKNOWN_REASON: DenialReasonCopy = {
  title: "We couldn't approve this application",
  explanation: "Your application didn't meet our lending criteria.",
  nextStep: "Contact our support team if you'd like to know more.",
};

export function describeDenialReasons(codes: string[]): DenialReasonCopy[] {
  const described = [...new Set(codes)].map((code) => DENIAL_REASONS[code] ?? UNKNOWN_REASON);
  return described.length > 0 ? [...new Set(described)] : [UNKNOWN_REASON];
}
