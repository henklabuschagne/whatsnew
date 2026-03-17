// ─── User Types ────────────────────────────────────────
export type UserRole = 'viewer' | 'admin';

export interface User {
  id: string;
  name: string;
  username: string;
  email: string;
  password: string;
  role: UserRole;
  token: string;
}

export interface PublicUser {
  id: string;
  name: string;
  role: UserRole;
}

// ─── Tag Types ─────────────────────────────────────────
export type TagType = 'module' | 'change-type';

export interface Tag {
  id: string;
  label: string;
  value: string;
  type: TagType;
  isActive: boolean;
}

// ─── Change Types ──────────────────────────────────────
export type ChangeType = 'bug-fix' | 'new-feature' | 'enhancement';

export interface Change {
  id: string;
  releaseId: string;
  title: string;
  description: string;
  changeType: ChangeType;
  moduleTags: string[];
  clientId?: string;
  ticketNumber?: string;
  devopsNumber?: string;
  createdAt: string;
  updatedAt: string;
}

// ─── Release Types ─────────────────────────────────────
export interface Release {
  id: string;
  version: string;
  releaseDate: string;
  title: string;
  description: string;
  isPublished: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ReleaseWithChanges extends Release {
  changes: Change[];
}

// ─── Client Types ──────────────────────────────────────
export interface Client {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

// ─── SQL Integration Types ─────────────────────────────
export interface SqlConnection {
  id: string;
  name: string;
  server: string;
  database: string;
  username?: string;
  password?: string;
  useIntegratedSecurity: boolean;
  isActive: boolean;
  createdAt: string;
}

export interface SqlQuery {
  id: string;
  connectionId: string;
  name: string;
  description?: string;
  queryText: string;
  isActive: boolean;
  lastExecuted?: string;
  createdAt: string;
}

// ─── Analytics Types ───────────────────────────────────
export interface DashboardSummary {
  totalReleases: number;
  totalChanges: number;
  publishedReleases: number;
  averageChangesPerRelease: number;
  lastReleaseDate: string;
  upcomingReleases: number;
  totalModules: number;
  releasesThisMonth: number;
  changesThisMonth: number;
  latestReleaseDate: string;
  latestVersion: string;
  clientRequestCount: number;
  ticketCount: number;
}

export interface TimelineEntry {
  month: string;
  monthName: string;
  year: number;
  releases: number;
  changes: number;
  releaseCount: number;
  totalChanges: number;
  bugFixes: number;
  newFeatures: number;
  enhancements: number;
}

export interface ModuleDistribution {
  tagId: string;
  moduleName: string;
  moduleValue: string;
  changeCount: number;
  bugFixes: number;
  newFeatures: number;
  enhancements: number;
  count: number;
  percentage: number;
}

export interface ChangeTypeDistribution {
  tagId: string;
  moduleName: string;
  moduleValue: string;
  changeCount: number;
  count: number;
  percentage: number;
}

export interface ClientDistribution {
  clientId: string | null;
  clientName: string;
  clientCode: string;
  changeCount: number;
  bugFixes: number;
  newFeatures: number;
  enhancements: number;
  count: number;
  percentage: number;
}

export interface TimeToActionMetrics {
  byChangeType: {
    changeType: string;
    label: string;
    averageTotalTime: number;
    averageDevTime: number;
    averageTestTime: number;
    averageReleaseTime: number;
    submittedToDeveloped: number;
    developedToTested: number;
    testedToReleased: number;
    count: number;
  }[];
  timeline: {
    month: string;
    monthName: string;
    bugFix: number;
    enhancement: number;
    newFeature: number;
  }[];
  overall: {
    averageTotalTime: number;
    fastestCompletion: number;
    slowestCompletion: number;
    medianTime: number;
  };
}

export interface RecentActivity {
  entityId: string;
  entityName: string;
  activityType: string;
  activityDate: string;
  description: string;
}

export interface ReleaseVelocity {
  averageTimeBetweenReleases: number;
  releasesPerMonth: number;
  trend: string;
}

export interface TopRelease {
  releaseId: string;
  version: string;
  changeCount: number;
  releaseDate: string;
}

// ─── Import/Export Types ───────────────────────────────
export interface ImportResult {
  importedReleases: number;
  importedChanges: number;
}
