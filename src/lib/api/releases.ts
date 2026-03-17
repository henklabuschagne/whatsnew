import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { Release, ReleaseWithChanges } from '../../types';

export async function getAllReleases(): Promise<ApiResult<Release[]>> {
  return mockApiCall(() => appStore.getAllReleases());
}

export async function getReleasesWithChanges(): Promise<ApiResult<ReleaseWithChanges[]>> {
  return mockApiCall(() => appStore.getReleasesWithChanges());
}

export async function getReleasesFiltered(filters: {
  searchTerm?: string;
  changeType?: string;
  moduleTag?: string;
  fromDate?: string;
  toDate?: string;
}): Promise<ApiResult<ReleaseWithChanges[]>> {
  return mockApiCall(() => appStore.getReleasesFiltered(filters));
}

export async function getReleaseById(id: string): Promise<ApiResult<Release>> {
  return mockApiCall(() => {
    const r = appStore.getReleaseById(id);
    if (!r) throw new Error(`Release ${id} not found`);
    return r;
  });
}

export async function createRelease(data: {
  version: string;
  releaseDate: string;
  title?: string;
  description?: string;
  isPublished?: boolean;
}): Promise<ApiResult<Release>> {
  if (!data.version.trim()) return errorResponse('VALIDATION_ERROR', 'Version is required');
  if (!data.releaseDate.trim()) return errorResponse('VALIDATION_ERROR', 'Release date is required');

  return mockApiCall(() => appStore.createRelease({
    version: data.version,
    releaseDate: data.releaseDate,
    title: data.title || '',
    description: data.description || '',
    isPublished: data.isPublished ?? false,
  }));
}

export async function updateRelease(id: string, data: Partial<Release>): Promise<ApiResult<Release>> {
  return mockApiCall(() => {
    const r = appStore.updateRelease(id, data);
    if (!r) throw new Error(`Release ${id} not found`);
    return r;
  });
}

export async function deleteRelease(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteRelease(id);
    if (!ok) throw new Error(`Release ${id} not found`);
    return true;
  });
}

export async function getStatistics(): Promise<ApiResult<ReturnType<typeof appStore.getStatistics>>> {
  return mockApiCall(() => appStore.getStatistics());
}

export async function getPopularTags(topN?: number): Promise<ApiResult<ReturnType<typeof appStore.getPopularTags>>> {
  return mockApiCall(() => appStore.getPopularTags(topN));
}

export async function searchChanges(query: string): Promise<ApiResult<ReturnType<typeof appStore.searchChanges>>> {
  return mockApiCall(() => appStore.searchChanges(query));
}
