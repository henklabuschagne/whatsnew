import { appStore } from '../appStore';
import { mockApiCall, errorResponse } from './config';
import type { ApiResult } from './types';
import type { SqlConnection, SqlQuery } from '../../types';

// ─── Connections ───────────────────────────────────────
export async function getAllConnections(): Promise<ApiResult<SqlConnection[]>> {
  return mockApiCall(() => appStore.getAllConnections());
}

export async function getConnectionById(id: string): Promise<ApiResult<SqlConnection>> {
  return mockApiCall(() => {
    const c = appStore.getConnectionById(id);
    if (!c) throw new Error(`Connection ${id} not found`);
    return c;
  });
}

export async function createConnection(data: {
  name: string;
  server: string;
  database: string;
  username?: string;
  password?: string;
  useIntegratedSecurity: boolean;
  isActive: boolean;
}): Promise<ApiResult<SqlConnection>> {
  if (!data.name.trim()) return errorResponse('VALIDATION_ERROR', 'Connection name is required');
  if (!data.server.trim()) return errorResponse('VALIDATION_ERROR', 'Server is required');
  if (!data.database.trim()) return errorResponse('VALIDATION_ERROR', 'Database is required');

  return mockApiCall(() => appStore.createConnection(data));
}

export async function updateConnection(id: string, data: Partial<SqlConnection>): Promise<ApiResult<SqlConnection>> {
  return mockApiCall(() => {
    const c = appStore.updateConnection(id, data);
    if (!c) throw new Error(`Connection ${id} not found`);
    return c;
  });
}

export async function deleteConnection(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteConnection(id);
    if (!ok) throw new Error(`Connection ${id} not found`);
    return true;
  });
}

export async function testConnection(data: {
  server: string;
  database: string;
  username?: string;
  password?: string;
  useIntegratedSecurity: boolean;
}): Promise<ApiResult<{ message: string }>> {
  return mockApiCall(() => ({ message: 'Connection test successful' }));
}

// ─── Queries ───────────────────────────────────────────
export async function getAllQueries(): Promise<ApiResult<SqlQuery[]>> {
  return mockApiCall(() => appStore.getAllQueries());
}

export async function getQueryById(id: string): Promise<ApiResult<SqlQuery>> {
  return mockApiCall(() => {
    const q = appStore.getQueryById(id);
    if (!q) throw new Error(`Query ${id} not found`);
    return q;
  });
}

export async function createQuery(data: {
  connectionId: string;
  name: string;
  description?: string;
  queryText: string;
  isActive: boolean;
}): Promise<ApiResult<SqlQuery>> {
  if (!data.name.trim()) return errorResponse('VALIDATION_ERROR', 'Query name is required');
  if (!data.queryText.trim()) return errorResponse('VALIDATION_ERROR', 'Query text is required');

  return mockApiCall(() => appStore.createQuery(data));
}

export async function updateQuery(id: string, data: Partial<SqlQuery>): Promise<ApiResult<SqlQuery>> {
  return mockApiCall(() => {
    const q = appStore.updateQuery(id, data);
    if (!q) throw new Error(`Query ${id} not found`);
    return q;
  });
}

export async function deleteQuery(id: string): Promise<ApiResult<boolean>> {
  return mockApiCall(() => {
    const ok = appStore.deleteQuery(id);
    if (!ok) throw new Error(`Query ${id} not found`);
    return true;
  });
}

export async function executeQuery(id: string): Promise<ApiResult<{ rows: any[]; message: string }>> {
  return mockApiCall(() => ({
    rows: [],
    message: 'Query executed successfully (mock). No data returned in mock mode.',
  }));
}
