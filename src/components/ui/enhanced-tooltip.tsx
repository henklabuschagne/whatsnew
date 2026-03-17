import { ReactNode, useState } from 'react';
import { X, Info } from 'lucide-react';
import { Button } from './button';

interface TooltipProps {
  content: ReactNode;
  children: ReactNode;
  position?: 'top' | 'bottom' | 'left' | 'right';
  showArrow?: boolean;
  className?: string;
  dismissible?: boolean;
  onDismiss?: () => void;
}

export function EnhancedTooltip({
  content,
  children,
  position = 'top',
  showArrow = true,
  className = '',
  dismissible = false,
  onDismiss,
}: TooltipProps) {
  const [isVisible, setIsVisible] = useState(false);
  const [isDismissed, setIsDismissed] = useState(false);

  const handleDismiss = () => {
    setIsDismissed(true);
    setIsVisible(false);
    onDismiss?.();
  };

  if (isDismissed) return <>{children}</>;

  const positionClasses = {
    top: 'bottom-full left-1/2 -translate-x-1/2 mb-2',
    bottom: 'top-full left-1/2 -translate-x-1/2 mt-2',
    left: 'right-full top-1/2 -translate-y-1/2 mr-2',
    right: 'left-full top-1/2 -translate-y-1/2 ml-2',
  };

  const arrowClasses = {
    top: 'top-full left-1/2 -translate-x-1/2 border-t-gray-900 border-l-transparent border-r-transparent border-b-transparent',
    bottom: 'bottom-full left-1/2 -translate-x-1/2 border-b-gray-900 border-l-transparent border-r-transparent border-t-transparent',
    left: 'left-full top-1/2 -translate-y-1/2 border-l-gray-900 border-t-transparent border-b-transparent border-r-transparent',
    right: 'right-full top-1/2 -translate-y-1/2 border-r-gray-900 border-t-transparent border-b-transparent border-l-transparent',
  };

  return (
    <div
      className="relative inline-block"
      onMouseEnter={() => setIsVisible(true)}
      onMouseLeave={() => setIsVisible(false)}
      onFocus={() => setIsVisible(true)}
      onBlur={() => setIsVisible(false)}
    >
      {children}
      {isVisible && (
        <div
          className={`absolute z-50 ${positionClasses[position]} ${className}`}
          role="tooltip"
          aria-live="polite"
        >
          <div className="bg-gray-900 text-white px-3 py-2 rounded-lg text-sm max-w-xs shadow-lg">
            <div className="flex items-start gap-2">
              <div className="flex-1">{content}</div>
              {dismissible && (
                <button
                  onClick={handleDismiss}
                  className="text-gray-400 hover:text-white transition-colors"
                  aria-label="Dismiss tooltip"
                >
                  <X className="w-4 h-4" />
                </button>
              )}
            </div>
          </div>
          {showArrow && (
            <div
              className={`absolute w-0 h-0 border-4 ${arrowClasses[position]}`}
              aria-hidden="true"
            />
          )}
        </div>
      )}
    </div>
  );
}

interface OnboardingTooltipProps {
  id: string;
  title: string;
  description: string;
  step?: number;
  totalSteps?: number;
  position?: 'top' | 'bottom' | 'left' | 'right';
  children: ReactNode;
  onNext?: () => void;
  onDismiss?: () => void;
  showOnce?: boolean;
}

export function OnboardingTooltip({
  id,
  title,
  description,
  step,
  totalSteps,
  position = 'bottom',
  children,
  onNext,
  onDismiss,
  showOnce = true,
}: OnboardingTooltipProps) {
  const [isVisible, setIsVisible] = useState(() => {
    if (!showOnce) return true;
    const dismissed = localStorage.getItem(`tooltip-dismissed-${id}`);
    return !dismissed;
  });

  const handleDismiss = () => {
    setIsVisible(false);
    if (showOnce) {
      localStorage.setItem(`tooltip-dismissed-${id}`, 'true');
    }
    onDismiss?.();
  };

  const handleNext = () => {
    handleDismiss();
    onNext?.();
  };

  if (!isVisible) return <>{children}</>;

  const positionClasses = {
    top: 'bottom-full left-1/2 -translate-x-1/2 mb-2',
    bottom: 'top-full left-1/2 -translate-x-1/2 mt-2',
    left: 'right-full top-1/2 -translate-y-1/2 mr-2',
    right: 'left-full top-1/2 -translate-y-1/2 ml-2',
  };

  return (
    <div className="relative inline-block">
      {children}
      <div
        className={`absolute z-50 ${positionClasses[position]}`}
        role="dialog"
        aria-labelledby={`tooltip-title-${id}`}
        aria-describedby={`tooltip-desc-${id}`}
      >
        <div className="bg-blue-600 text-white p-4 rounded-lg shadow-xl max-w-sm animate-in fade-in slide-in-from-top-2">
          <div className="flex items-start gap-3 mb-3">
            <Info className="w-5 h-5 mt-0.5 flex-shrink-0" aria-hidden="true" />
            <div className="flex-1">
              <h3 id={`tooltip-title-${id}`} className="font-semibold mb-1">
                {title}
              </h3>
              <p id={`tooltip-desc-${id}`} className="text-sm text-blue-50">
                {description}
              </p>
            </div>
            <button
              onClick={handleDismiss}
              className="text-blue-200 hover:text-white transition-colors"
              aria-label="Dismiss tooltip"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
          <div className="flex items-center justify-between">
            {step && totalSteps && (
              <span className="text-xs text-blue-200">
                {step} of {totalSteps}
              </span>
            )}
            <div className="flex gap-2 ml-auto">
              <Button
                variant="outline"
                size="sm"
                onClick={handleDismiss}
                className="bg-transparent border-white text-white hover:bg-white/10"
              >
                {onNext ? 'Skip' : 'Got it'}
              </Button>
              {onNext && (
                <Button size="sm" onClick={handleNext} className="bg-white text-blue-600 hover:bg-blue-50">
                  Next
                </Button>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
