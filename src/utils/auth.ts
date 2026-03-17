// LEGACY: This file is superseded by /lib/appStore.ts and /lib/api/auth.ts
// Kept for backward compatibility - prefer using useAppStore hook instead.

import type { PublicUser } from '../types';

const AUTH_STORAGE_KEY = 'whats-new-current-user';
const TOKEN_STORAGE_KEY = 'auth_token';

export const authUtils = {
  getCurrentUser(): PublicUser | null {
    const data = localStorage.getItem(AUTH_STORAGE_KEY);
    return data ? JSON.parse(data) : null;
  },

  setCurrentUser(user: PublicUser): void {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));
  },

  logout(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    localStorage.removeItem(TOKEN_STORAGE_KEY);
  },

  getMockUsers(): PublicUser[] {
    return [
      { id: '1', name: 'Admin User', role: 'admin' },
      { id: '2', name: 'John Viewer', role: 'viewer' },
    ];
  }
};
