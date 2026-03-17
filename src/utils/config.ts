// Application configuration

// Helper to safely access environment variables
const getEnv = (key: string, defaultValue: string = ''): string => {
  if (typeof import.meta !== 'undefined' && import.meta.env) {
    return import.meta.env[key] || defaultValue;
  }
  return defaultValue;
};

export const config = {
  // API Configuration
  apiBaseUrl: getEnv('VITE_API_BASE_URL', 'http://localhost:5000/api'),
  
  // Environment
  isDevelopment: getEnv('VITE_ENV', 'development') === 'development' || 
                 (typeof import.meta !== 'undefined' && import.meta.env?.DEV),
  isProduction: getEnv('VITE_ENV', 'development') === 'production' || 
                (typeof import.meta !== 'undefined' && import.meta.env?.PROD),
  
  // Feature Flags
  enableMockData: getEnv('VITE_ENABLE_MOCK_DATA', 'true') === 'true',
  
  // API Timeouts
  apiTimeout: 30000, // 30 seconds
  
  // Retry Configuration
  maxRetries: 3,
  retryDelay: 1000, // 1 second
};

// Check if backend is available
export const checkBackendHealth = async (): Promise<boolean> => {
  try {
    const response = await fetch(`${config.apiBaseUrl}/health`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return response.ok;
  } catch (error) {
    return false;
  }
};