export type ChangeType = 'bug-fix' | 'new-feature' | 'enhancement';
export type ModuleTag = 'import' | 'export' | 'packs' | 'systems' | 'security' | 'reports' | 'publisher' | 'dashboard' | string;

export interface Change {
  id: string;
  description: string;
  changeType: ChangeType;
  moduleTags: ModuleTag[];
  clientId?: string;
  ticketNumber?: string;
  devopsNumber?: string;
}

export interface Release {
  id: string;
  version: string;
  releaseDate: string;
  changes: Change[];
}

export interface CustomTag {
  id: string;
  label: string;
  value: string;
  type: 'module' | 'changeType';
  color?: string;
}

export interface SQLIntegration {
  id: string;
  name: string;
  host: string;
  port: string;
  database: string;
  username: string;
  password: string;
  query: string;
  enabled: boolean;
  lastSync?: string;
}