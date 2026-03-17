// LEGACY: This hook is superseded by useAppStore('changes')
// Kept for backward compatibility.

import { useAppStore } from './useAppStore';

export function useChanges() {
  const { changes, actions } = useAppStore('changes');

  return {
    changes,
    loading: false,
    error: null,
    createChange: actions.createChange,
    updateChange: actions.updateChange,
    deleteChange: actions.deleteChange,
    refresh: () => {},
  };
}
