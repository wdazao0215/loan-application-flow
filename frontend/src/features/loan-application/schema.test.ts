import { describe, expect, it } from "vitest";
import {
  EMPTY_LOAN_APPLICATION,
  loanApplicationSchema,
  validateField,
  validateLoanApplication,
  type LoanApplicationInput,
} from "./schema";

const validInput: LoanApplicationInput = {
  firstName: " Ada ",
  lastName: "Lovelace",
  companyName: "Analytical Engines LLC",
  street: "100 Congress Ave",
  city: "Austin",
  state: "TX",
  zipCode: "78701",
  requestedAmount: "25,000.50",
  ssn: "521-45-7788",
};

describe("loan application schema", () => {
  it("parses a complete application into the values sent to the API", () => {
    const application = loanApplicationSchema.parse(validInput);

    expect(application.firstName).toBe("Ada");
    expect(application.requestedAmount).toBe(25000.5);
  });

  it("reports one message per missing field", () => {
    const errors = validateLoanApplication(EMPTY_LOAN_APPLICATION);

    expect(Object.keys(errors).sort()).toEqual(
      ["city", "companyName", "firstName", "lastName", "requestedAmount", "ssn", "state", "street", "zipCode"].sort(),
    );
    expect(errors.state).toBe("Select your state");
    expect(errors.requestedAmount).toBe("Enter the amount you want to borrow");
  });

  it.each(["123-45-6789", "123456789"])("accepts the SSN %s", (ssn) => {
    expect(validateField("ssn", ssn)).toBeUndefined();
  });

  it.each(["12345", "123-45-678", "abc-de-fghi"])("rejects the SSN %s", (ssn) => {
    expect(validateField("ssn", ssn)).toBe("Enter your 9-digit Social Security number");
  });

  it.each([
    ["0", "Enter an amount greater than $0"],
    ["10.005", "Enter an amount in dollars, like 25,000"],
    ["-5", "Enter an amount in dollars, like 25,000"],
    ["2000000000", "Enter an amount up to $1,000,000,000"],
  ])("rejects the amount %s", (amount, message) => {
    expect(validateField("requestedAmount", amount)).toBe(message);
  });

  it("only accepts US state codes", () => {
    expect(validateField("state", "NY")).toBeUndefined();
    expect(validateField("state", "New York")).toBe("Select your state");
  });
});
