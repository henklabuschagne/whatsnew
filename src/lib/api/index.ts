import * as auth from './auth';
import * as releases from './releases';
import * as changes from './changes';
import * as tags from './tags';
import * as clients from './clients';
import * as analytics from './analytics';
import * as integrations from './integrations';
import * as importExport from './importExport';

export const api = {
  auth,
  releases,
  changes,
  tags,
  clients,
  analytics,
  integrations,
  importExport,
};

export type { ApiResult, ApiError, PaginatedResult } from './types';
export { apiConfig } from './config';
