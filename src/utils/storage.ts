// LEGACY: This file is superseded by /lib/appStore.ts
// Kept for backward compatibility - prefer using useAppStore hook instead.

import type { Tag } from '../types';

const STORAGE_KEY = 'whats-new-releases';
const TAGS_STORAGE_KEY = 'whats-new-tags';

export const storageUtils = {
  getReleases(): any[] {
    const data = localStorage.getItem(STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  },

  saveReleases(releases: any[]): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(releases));
  },

  // Tag management
  getTags(): Tag[] {
    const data = localStorage.getItem(TAGS_STORAGE_KEY);
    return data ? JSON.parse(data) : this.getDefaultTags();
  },

  getDefaultTags(): Tag[] {
    return [
      { id: '1', label: 'Import', value: 'import', type: 'module', isActive: true },
      { id: '2', label: 'Export', value: 'export', type: 'module', isActive: true },
      { id: '3', label: 'Packs', value: 'packs', type: 'module', isActive: true },
      { id: '4', label: 'Systems', value: 'systems', type: 'module', isActive: true },
      { id: '5', label: 'Security', value: 'security', type: 'module', isActive: true },
      { id: '6', label: 'Reports', value: 'reports', type: 'module', isActive: true },
      { id: '7', label: 'Publisher', value: 'publisher', type: 'module', isActive: true },
      { id: '8', label: 'Dashboard', value: 'dashboard', type: 'module', isActive: true },
    ];
  },

  saveTags(tags: Tag[]): void {
    localStorage.setItem(TAGS_STORAGE_KEY, JSON.stringify(tags));
  },
};
