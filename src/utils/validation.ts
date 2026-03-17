export interface ValidationResult {
  isValid: boolean;
  errors: Record<string, string>;
}

export const validators = {
  required: (value: any, fieldName: string): string | null => {
    if (!value || (typeof value === 'string' && value.trim() === '')) {
      return `${fieldName} is required`;
    }
    return null;
  },

  minLength: (value: string, min: number, fieldName: string): string | null => {
    if (value && value.length < min) {
      return `${fieldName} must be at least ${min} characters`;
    }
    return null;
  },

  maxLength: (value: string, max: number, fieldName: string): string | null => {
    if (value && value.length > max) {
      return `${fieldName} must be no more than ${max} characters`;
    }
    return null;
  },

  email: (value: string, fieldName: string): string | null => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (value && !emailRegex.test(value)) {
      return `${fieldName} must be a valid email address`;
    }
    return null;
  },

  url: (value: string, fieldName: string): string | null => {
    try {
      if (value) {
        new URL(value);
      }
      return null;
    } catch {
      return `${fieldName} must be a valid URL`;
    }
  },

  version: (value: string, fieldName: string): string | null => {
    const versionRegex = /^\d+\.\d+(\.\d+)?$/;
    if (value && !versionRegex.test(value)) {
      return `${fieldName} must be in format X.Y or X.Y.Z (e.g., 1.0 or 1.0.0)`;
    }
    return null;
  },

  date: (value: any, fieldName: string): string | null => {
    if (value && isNaN(Date.parse(value))) {
      return `${fieldName} must be a valid date`;
    }
    return null;
  },

  port: (value: number | string, fieldName: string): string | null => {
    const port = typeof value === 'string' ? parseInt(value, 10) : value;
    if (isNaN(port) || port < 1 || port > 65535) {
      return `${fieldName} must be a valid port number (1-65535)`;
    }
    return null;
  }
};

export function validateField(
  value: any,
  rules: Array<(value: any) => string | null>
): string | null {
  for (const rule of rules) {
    const error = rule(value);
    if (error) {
      return error;
    }
  }
  return null;
}

export function validateForm<T extends Record<string, any>>(
  data: T,
  schema: Record<keyof T, Array<(value: any) => string | null>>
): ValidationResult {
  const errors: Record<string, string> = {};
  let isValid = true;

  for (const field in schema) {
    const error = validateField(data[field], schema[field]);
    if (error) {
      errors[field] = error;
      isValid = false;
    }
  }

  return { isValid, errors };
}
