import { mockApiCall } from './config';
import type { ApiResult } from './types';
import type { ImportResult } from '../../types';

export async function importExcel(file: File): Promise<ApiResult<ImportResult>> {
  return mockApiCall(() => ({
    importedReleases: 0,
    importedChanges: 0,
  }));
}

export async function exportExcel(): Promise<ApiResult<Blob>> {
  return mockApiCall(() =>
    new Blob(['Mock Excel Data'], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    })
  );
}

export async function downloadTemplate(): Promise<ApiResult<Blob>> {
  return mockApiCall(() =>
    new Blob(['Mock Template Data'], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    })
  );
}
