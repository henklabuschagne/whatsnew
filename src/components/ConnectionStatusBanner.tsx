import { useState, useEffect } from 'react';
import { WifiOff, Wifi, AlertCircle, X } from 'lucide-react';
import { config } from '../utils/config';
import { Button } from './ui/button';

export function ConnectionStatusBanner() {
  const [showBanner, setShowBanner] = useState(false);
  const [dismissed, setDismissed] = useState(false);

  useEffect(() => {
    // Show banner if using mock data
    if (config.enableMockData && !dismissed) {
      setShowBanner(true);
    }
  }, [dismissed]);

  if (!showBanner) return null;

  return (
    <div className="bg-blue-50 border-b border-blue-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-3">
        <div className="flex items-center justify-between gap-4">
          <div className="flex items-center gap-3">
            <Wifi className="w-5 h-5 text-blue-600 flex-shrink-0" aria-hidden="true" />
            <div className="flex-1">
              <p className="text-sm text-blue-800">
                <span className="font-semibold">Demo Mode Active.</span>{' '}
                Running with sample data. Perfect for exploring features! Connect to backend for real data.
              </p>
            </div>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => {
              setDismissed(true);
              setShowBanner(false);
            }}
            className="text-blue-600 hover:text-blue-700 hover:bg-blue-100"
            aria-label="Dismiss notification"
          >
            <X className="w-4 h-4" />
          </Button>
        </div>
      </div>
    </div>
  );
}

// Real-time connection indicator
export function ConnectionIndicator() {
  const [isOnline, setIsOnline] = useState(navigator.onLine);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  if (isOnline && !config.enableMockData) return null;

  return (
    <div className="fixed bottom-4 left-4 z-50">
      <div
        className={`flex items-center gap-2 px-3 py-2 rounded-lg shadow-lg ${
          isOnline
            ? 'bg-blue-100 text-blue-800 border border-blue-300'
            : 'bg-red-100 text-red-800 border border-red-300'
        }`}
        role="status"
        aria-live="polite"
      >
        {isOnline ? (
          <>
            <Wifi className="w-4 h-4" aria-hidden="true" />
            <span className="text-sm font-medium">Demo Mode</span>
          </>
        ) : (
          <>
            <WifiOff className="w-4 h-4" aria-hidden="true" />
            <span className="text-sm font-medium">Offline</span>
          </>
        )}
      </div>
    </div>
  );
}