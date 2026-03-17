import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { Change } from '../../types';

export async function getChangesByReleaseId(releaseId: string): Promise<ApiResult<Change[]>> {
  return mockApiCall(() => appStore.getChangesByReleaseId(releaseId));
}

export async function getChangeById(id: string): Promise<ApiResult<Change>> {
  return mockApiCall(() => {
    const c = appStore.getChangeById(id);
    if (!c) throw new Error(`Change ${id} not found`);
    return c;
  });
}

export async function createChange(data: {
  releaseId: string;
  title: string;
  description: string;
  changeType: string;
  moduleTags: string[];
  clientId?: string;
  ticketNumber?: string;
  devopsNumber?: string;
}): Promise<ApiResult<Change>> {
  if (!data.title.trim()) return errorResponse('VALIDATION_ERROR', 'Title is required');
  if (!data.description.trim()) return errorResponse('VALIDATION_ERROR', 'Description is required');
  if (!data.releaseId) return errorResponse('VALIDATION_ERROR', 'Release ID is required');

  return mockApiCall(() => appStore.createChange({
    releaseId: data.releaseId,
    title: data.title,
    description: data.description,
    changeType: data.changeType as Change['changeType'],
    moduleTags: data.moduleTags,
    clientId: data.clientId,
    ticketNumber: data.ticketNumber,
    devopsNumber: data.devopsNumber,
  }));
}

export async function updateChange(id: string, data: Partial<Change>): Promise<ApiResult<Change>> {
  return mockApiCall(() => {
    const c = appStore.updateChange(id, data);
    if (!c) throw new Error(`Change ${id} not found`);
    return c;
  });
}

export async function deleteChange(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteChange(id);
    if (!ok) throw new Error(`Change ${id} not found`);
    return true;
  });
}
