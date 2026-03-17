// LEGACY: This hook is superseded by useAppStore('releases', 'changes')
// Kept for backward compatibility.

import { useAppStore } from './useAppStore';

export function useReleases() {
  const { releases, reads, actions } = useAppStore('releases', 'changes');

  return {
    releases: reads.getReleasesWithChanges(),
    loading: false,
    error: null,
    createRelease: actions.createRelease,
    updateRelease: actions.updateRelease,
    deleteRelease: actions.deleteRelease,
    refresh: () => {},
  };
}
