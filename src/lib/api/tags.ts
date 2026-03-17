import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { Tag } from '../../types';

export async function getAllTags(type?: string): Promise<ApiResult<Tag[]>> {
  return mockApiCall(() => type ? appStore.getTagsByType(type) : appStore.getAllTags());
}

export async function getTagById(id: string): Promise<ApiResult<Tag>> {
  return mockApiCall(() => {
    const t = appStore.getTagById(id);
    if (!t) throw new Error(`Tag ${id} not found`);
    return t;
  });
}

export async function createTag(data: {
  label: string;
  value: string;
  type: string;
}): Promise<ApiResult<Tag>> {
  if (!data.label.trim()) return errorResponse('VALIDATION_ERROR', 'Label is required');
  if (!data.value.trim()) return errorResponse('VALIDATION_ERROR', 'Value is required');

  return mockApiCall(() => appStore.createTag({
    label: data.label,
    value: data.value,
    type: data.type as Tag['type'],
    isActive: true,
  }));
}

export async function updateTag(id: string, data: Partial<Tag>): Promise<ApiResult<Tag>> {
  return mockApiCall(() => {
    const t = appStore.updateTag(id, data);
    if (!t) throw new Error(`Tag ${id} not found`);
    return t;
  });
}

export async function deleteTag(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteTag(id);
    if (!ok) throw new Error(`Tag ${id} not found`);
    return true;
  });
}
