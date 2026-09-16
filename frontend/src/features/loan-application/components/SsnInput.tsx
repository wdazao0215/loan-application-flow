"use client";

import { useState } from "react";
import { formatSsn } from "../formatting";
import { controlClassName, type ControlProps } from "./FormField";

type SsnInputProps = ControlProps & {
  value: string;
  onChange: (value: string) => void;
  onBlur: () => void;
};

export function SsnInput({ value, onChange, onBlur, ...controlProps }: SsnInputProps) {
  const [isVisible, setIsVisible] = useState(false);

  return (
    <div className="relative">
      <input
        {...controlProps}
        type="text"
        inputMode="numeric"
        autoComplete="off"
        spellCheck={false}
        placeholder="123-45-6789"
        value={value}
        onChange={(event) => onChange(formatSsn(event.target.value))}
        onBlur={onBlur}
        className={`${controlClassName(controlProps["aria-invalid"])} pr-20 font-mono tracking-wider ${
          isVisible ? "" : "[-webkit-text-security:disc]"
        }`}
      />
      <button
        type="button"
        onClick={() => setIsVisible((visible) => !visible)}
        aria-controls={controlProps.id}
        aria-pressed={isVisible}
        className="absolute inset-y-1.5 right-1.5 rounded-md px-3 text-sm font-medium text-brand-700 hover:bg-brand-50 focus-visible:outline-2 focus-visible:outline-brand-600"
      >
        {isVisible ? "Hide" : "Show"}
      </button>
    </div>
  );
}
