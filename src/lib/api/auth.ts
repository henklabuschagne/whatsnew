import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { PublicUser } from '../../types';

export async function login(username: string, password: string): Promise<ApiResult<PublicUser>> {
  if (!username.trim()) return errorResponse('VALIDATION_ERROR', 'Username is required');
  if (!password.trim()) return errorResponse('VALIDATION_ERROR', 'Password is required');

  return mockApiCall(() => {
    const user = appStore.login(username, password);
    if (!user) throw new Error('Invalid credentials');
    return user;
  });
}

export async function loginAs(user: PublicUser): Promise<ApiResult<PublicUser>> {
  return mockApiCall(() => appStore.loginAs(user));
}

export async function logout(): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    appStore.logout();
    return true;
  });
}

export async function getCurrentUser(): Promise<ApiResult<PublicUser | null>> {
  return mockApiCall(() => appStore.getCurrentUser());
}

export async function getUsers(): Promise<ApiResult<PublicUser[]>> {
  return mockApiCall(() => appStore.getUsers());
}
