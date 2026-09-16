import { describe, expect, it } from "vitest";
import { formatAmount, formatSsn, sanitizeAmount } from "./formatting";

describe("formatSsn", () => {
  it.each([
    ["123", "123"],
    ["1234", "123-4"],
    ["123456", "123-45-6"],
    ["123456789", "123-45-6789"],
    ["123-45-67890", "123-45-6789"],
    ["12a3", "123"],
  ])("formats %s as %s", (input, expected) => {
    expect(formatSsn(input)).toBe(expected);
  });
});

describe("sanitizeAmount", () => {
  it.each([
    ["$25,000", "25000"],
    ["1000.999", "1000.99"],
    ["10.5.5", "10.55"],
  ])("keeps only a plain amount from %s", (input, expected) => {
    expect(sanitizeAmount(input)).toBe(expected);
  });
});

describe("formatAmount", () => {
  it.each([
    ["25000", "25,000"],
    ["1234567.5", "1,234,567.50"],
    ["abc", "abc"],
  ])("formats %s as %s", (input, expected) => {
    expect(formatAmount(input)).toBe(expected);
  });
});
