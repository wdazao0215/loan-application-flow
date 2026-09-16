const SSN_DIGITS = 9;

export function formatSsn(input: string): string {
  const digits = input.replace(/\D/g, "").slice(0, SSN_DIGITS);

  if (digits.length <= 3) {
    return digits;
  }

  if (digits.length <= 5) {
    return `${digits.slice(0, 3)}-${digits.slice(3)}`;
  }

  return `${digits.slice(0, 3)}-${digits.slice(3, 5)}-${digits.slice(5)}`;
}

export function sanitizeAmount(input: string): string {
  const [whole = "", ...fractions] = input.replace(/[^\d.]/g, "").split(".");
  return fractions.length === 0 ? whole : `${whole}.${fractions.join("").slice(0, 2)}`;
}

export function formatAmount(input: string): string {
  const plain = input.replace(/,/g, "");
  if (!/^\d+(\.\d{1,2})?$/.test(plain)) {
    return input;
  }

  const [whole, cents] = plain.split(".");
  const groupedWhole = Number(whole).toLocaleString("en-US");
  return cents === undefined ? groupedWhole : `${groupedWhole}.${cents.padEnd(2, "0")}`;
}
