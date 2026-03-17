import { toast } from "sonner@2.0.3";

export interface ApiError {
  message: string;
  statusCode?: number;
  errors?: Record<string, string[]>;
}

export class ErrorHandler {
  static handle(error: any, fallbackMessage: string = 'An error occurred'): void {
    console.error('Error:', error);

    if (error.response) {
      // API error response
      const apiError = error.response.data as ApiError;
      
      if (apiError.errors) {
        // Validation errors
        Object.values(apiError.errors).forEach(messages => {
          messages.forEach(msg => toast.error(msg));
        });
      } else if (apiError.message) {
        toast.error(apiError.message);
      } else {
        toast.error(fallbackMessage);
      }
    } else if (error.request) {
      // Network error
      toast.error('Network error. Please check your connection.');
    } else {
      // Other errors
      toast.error(error.message || fallbackMessage);
    }
  }

  static handleValidation(errors: Record<string, string>): void {
    Object.values(errors).forEach(message => {
      toast.error(message);
    });
  }

  static success(message: string): void {
    toast.success(message);
  }

  static info(message: string): void {
    toast.info(message);
  }

  static warning(message: string): void {
    toast.warning(message);
  }
}

// HTTP Status Code helpers
export const isUnauthorized = (error: any): boolean => {
  return error?.response?.status === 401;
};

export const isForbidden = (error: any): boolean => {
  return error?.response?.status === 403;
};

export const isNotFound = (error: any): boolean => {
  return error?.response?.status === 404;
};

export const isValidationError = (error: any): boolean => {
  return error?.response?.status === 400;
};

export const isServerError = (error: any): boolean => {
  return error?.response?.status >= 500;
};
