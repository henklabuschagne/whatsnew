import { useEffect, useState } from 'react';

interface A11yAnnouncerProps {
  message: string;
  politeness?: 'polite' | 'assertive';
}

/**
 * Accessibility announcer for screen readers
 * Use this component to announce dynamic changes to screen reader users
 */
export function A11yAnnouncer({ message, politeness = 'polite' }: A11yAnnouncerProps) {
  const [announcement, setAnnouncement] = useState('');

  useEffect(() => {
    if (message) {
      // Clear and set to ensure announcement is read
      setAnnouncement('');
      setTimeout(() => setAnnouncement(message), 100);
    }
  }, [message]);

  return (
    <div
      role="status"
      aria-live={politeness}
      aria-atomic="true"
      className="sr-only"
    >
      {announcement}
    </div>
  );
}

/**
 * Hook for making announcements
 */
export function useA11yAnnounce() {
  const [message, setMessage] = useState('');

  const announce = (text: string) => {
    setMessage(text);
    // Clear after announcement
    setTimeout(() => setMessage(''), 1000);
  };

  return { message, announce };
}
