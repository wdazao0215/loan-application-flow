import { describe, expect, it } from "vitest";
import { describeDenialReasons } from "./denial-reasons";

describe("describeDenialReasons", () => {
  it("describes every known reason once", () => {
    const reasons = describeDenialReasons(["state-not-served", "identity-not-verified", "state-not-served"]);

    expect(reasons.map((reason) => reason.title)).toEqual([
      "We don't lend in your state yet",
      "We couldn't verify your identity",
    ]);
  });

  it("falls back to a generic reason for unknown or missing codes", () => {
    expect(describeDenialReasons(["something-new"])[0].title).toBe("We couldn't approve this application");
    expect(describeDenialReasons([])[0].title).toBe("We couldn't approve this application");
  });
});
