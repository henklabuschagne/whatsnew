// LEGACY: This hook is superseded by useAppStore('tags')
// Kept for backward compatibility.

import { useAppStore } from './useAppStore';

export function useTags() {
  const { tags, actions } = useAppStore('tags');

  return {
    tags,
    loading: false,
    error: null,
    createTag: actions.createTag,
    updateTag: actions.updateTag,
    deleteTag: actions.deleteTag,
    refresh: () => {},
  };
}
