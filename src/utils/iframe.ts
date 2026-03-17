// Utility functions for iframe embedding support

/**
 * Checks if the application is running inside an iframe
 */
export const isInIframe = (): boolean => {
  try {
    return window.self !== window.top;
  } catch (e) {
    // If we get a SecurityError, we're definitely in an iframe
    return true;
  }
};

/**
 * Gets the parent window origin if running in an iframe
 * Returns null if not in an iframe or if access is denied
 */
export const getParentOrigin = (): string | null => {
  if (!isInIframe()) {
    return null;
  }

  try {
    return document.referrer ? new URL(document.referrer).origin : null;
  } catch (e) {
    return null;
  }
};

/**
 * Sends a message to the parent window if running in an iframe
 */
export const sendMessageToParent = (message: any): void => {
  if (!isInIframe()) {
    return;
  }

  try {
    window.parent.postMessage(message, '*');
  } catch (e) {
    console.error('Failed to send message to parent:', e);
  }
};

/**
 * Listens for messages from the parent window
 */
export const onMessageFromParent = (
  callback: (event: MessageEvent) => void
): (() => void) => {
  const handler = (event: MessageEvent) => {
    // Validate the message if needed
    callback(event);
  };

  window.addEventListener('message', handler);

  // Return cleanup function
  return () => {
    window.removeEventListener('message', handler);
  };
};

/**
 * Notifies parent window that the app is ready (useful for iframe integration)
 */
export const notifyParentReady = (): void => {
  sendMessageToParent({
    type: 'WHATS_NEW_READY',
    timestamp: new Date().toISOString(),
  });
};

/**
 * Notifies parent window of navigation changes
 */
export const notifyParentNavigation = (path: string): void => {
  sendMessageToParent({
    type: 'WHATS_NEW_NAVIGATION',
    path,
    timestamp: new Date().toISOString(),
  });
};

/**
 * Requests full screen from parent if in iframe
 */
export const requestFullscreen = (): void => {
  sendMessageToParent({
    type: 'WHATS_NEW_REQUEST_FULLSCREEN',
    timestamp: new Date().toISOString(),
  });
};
