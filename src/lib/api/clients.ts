import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { Client } from '../../types';

export async function getAllClients(): Promise<ApiResult<Client[]>> {
  return mockApiCall(() => appStore.getAllClients());
}

export async function getClientById(id: string): Promise<ApiResult<Client>> {
  return mockApiCall(() => {
    const c = appStore.getClientById(id);
    if (!c) throw new Error(`Client ${id} not found`);
    return c;
  });
}

export async function createClient(data: {
  name: string;
  code: string;
  description?: string;
}): Promise<ApiResult<Client>> {
  if (!data.name.trim()) return errorResponse('VALIDATION_ERROR', 'Client name is required');
  if (!data.code.trim()) return errorResponse('VALIDATION_ERROR', 'Client code is required');

  return mockApiCall(() => appStore.createClient({
    name: data.name,
    code: data.code,
    description: data.description,
    isActive: true,
  }));
}

export async function updateClient(id: string, data: Partial<Client>): Promise<ApiResult<Client>> {
  return mockApiCall(() => {
    const c = appStore.updateClient(id, data);
    if (!c) throw new Error(`Client ${id} not found`);
    return c;
  });
}

export async function deleteClient(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteClient(id);
    if (!ok) throw new Error(`Client ${id} not found`);
    return true;
  });
}
