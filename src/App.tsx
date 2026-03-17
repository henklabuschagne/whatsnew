import { useEffect } from 'react';
import { RouterProvider } from 'react-router';
import { router } from './utils/routes';
import { Toaster } from './components/ui/sonner';
import { LoginPage } from './components/LoginPage';
import { ErrorBoundary } from './components/ErrorBoundary';
import { SkipLinks } from './components/SkipLinks';
import { useAppStore } from './hooks/useAppStore';
import { isInIframe, notifyParentReady } from './utils/iframe';

function App() {
  const { currentUser, reads, actions } = useAppStore('auth');

  useEffect(() => {
    if (isInIframe()) {
      notifyParentReady();
    }
  }, []);

  if (!currentUser) {
    return (
      <ErrorBoundary>
        <SkipLinks />
        <LoginPage onLogin={() => {}} />
        <Toaster position="bottom-right" />
      </ErrorBoundary>
    );
  }

  return (
    <ErrorBoundary>
      <SkipLinks />
      <RouterProvider router={router} />
      <Toaster position="bottom-right" />
    </ErrorBoundary>
  );
}

export default App;