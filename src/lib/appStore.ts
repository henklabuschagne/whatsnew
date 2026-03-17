// ─── Centralized Data Store ────────────────────────────
// All application state lives here. ONLY the API layer and useAppStore hook may import this.

import type {
  User, PublicUser, Tag, Change, Release, ReleaseWithChanges, Client,
  SqlConnection, SqlQuery, DashboardSummary, TimelineEntry,
  ModuleDistribution, ChangeTypeDistribution, ClientDistribution,
  TimeToActionMetrics, RecentActivity, ReleaseVelocity, TopRelease,
} from '../types';

// ─── Subscriber System ─────────────────────────────────
export type Slice = 'auth' | 'releases' | 'changes' | 'tags' | 'clients' | 'connections' | 'queries';
type Listener = () => void;

const subscribers: Record<Slice, Set<Listener>> = {
  auth: new Set(),
  releases: new Set(),
  changes: new Set(),
  tags: new Set(),
  clients: new Set(),
  connections: new Set(),
  queries: new Set(),
};

function notify(slice: Slice) {
  persist(slice);
  subscribers[slice].forEach(fn => fn());
}

// ─── localStorage Persistence ──────────────────────────
const STORAGE_PREFIX = 'whatsnew_';

function persist(slice: Slice) {
  try {
    let data: any;
    switch (slice) {
      case 'auth': data = currentUser; break;
      case 'releases': data = releases; break;
      case 'changes': data = changes; break;
      case 'tags': data = tags; break;
      case 'clients': data = clients; break;
      case 'connections': data = sqlConnections; break;
      case 'queries': data = sqlQueries; break;
    }
    if (data !== undefined) {
      localStorage.setItem(STORAGE_PREFIX + slice, JSON.stringify(data));
    }
  } catch { /* localStorage full or unavailable */ }
}

function hydrate<T>(slice: Slice, fallback: T): T {
  try {
    const raw = localStorage.getItem(STORAGE_PREFIX + slice);
    if (raw) {
      return JSON.parse(raw);
    }
  } catch { /* corrupted data */ }
  return fallback;
}

function clearAllPersisted() {
  const keys = Object.keys(localStorage).filter(k => k.startsWith(STORAGE_PREFIX));
  keys.forEach(k => localStorage.removeItem(k));
  // Also clear legacy auth key
  localStorage.removeItem('whats-new-current-user');
  localStorage.removeItem('auth_token');
}

// ─── ID Generator ──────────────────────────────────────
let idCounter = 100;
function generateId(): string {
  return String(++idCounter);
}

// ─── Seed Data ─────────────────────────────────────────
const seedUsers: User[] = [
  {
    id: '1', username: 'admin', password: 'admin123', name: 'Admin User',
    email: 'admin@example.com', role: 'admin', token: 'mock-jwt-token-admin-12345',
  },
  {
    id: '2', username: 'viewer', password: 'viewer123', name: 'John Viewer',
    email: 'viewer@example.com', role: 'viewer', token: 'mock-jwt-token-viewer-67890',
  },
];

const seedTags: Tag[] = [
  { id: '1', label: 'Import', value: 'import', type: 'module', isActive: true },
  { id: '2', label: 'Export', value: 'export', type: 'module', isActive: true },
  { id: '3', label: 'Packs', value: 'packs', type: 'module', isActive: true },
  { id: '4', label: 'Systems', value: 'systems', type: 'module', isActive: true },
  { id: '5', label: 'Security', value: 'security', type: 'module', isActive: true },
  { id: '6', label: 'Reports', value: 'reports', type: 'module', isActive: true },
  { id: '7', label: 'Publisher', value: 'publisher', type: 'module', isActive: true },
  { id: '8', label: 'Dashboard', value: 'dashboard', type: 'module', isActive: true },
  { id: '9', label: 'Bug Fix', value: 'bug-fix', type: 'change-type', isActive: true },
  { id: '10', label: 'New Feature', value: 'new-feature', type: 'change-type', isActive: true },
  { id: '11', label: 'Enhancement', value: 'enhancement', type: 'change-type', isActive: true },
];

const seedChanges: Change[] = [
  { id: '1', releaseId: '1', title: 'Improved data import performance by 40%', description: 'Optimized the import process with batch processing and async operations to handle larger datasets more efficiently. Reduced memory footprint by 30%.', changeType: 'enhancement', moduleTags: ['import'], clientId: '1', ticketNumber: 'TICKET-12345', devopsNumber: 'DEVOPS-13456', createdAt: '2024-12-01T10:00:00Z', updatedAt: '2024-12-01T10:00:00Z' },
  { id: '2', releaseId: '1', title: 'Fixed export formatting issues with Excel files', description: 'Resolved a bug where special characters (UTF-8) were not properly encoded in Excel exports, causing data corruption.', changeType: 'bug-fix', moduleTags: ['export'], clientId: '2', ticketNumber: 'TICKET-12346', createdAt: '2024-12-01T10:30:00Z', updatedAt: '2024-12-01T10:30:00Z' },
  { id: '3', releaseId: '1', title: 'New dashboard analytics widgets', description: 'Added customizable widgets for real-time data visualization including line charts, bar charts, and KPI indicators.', changeType: 'new-feature', moduleTags: ['dashboard'], devopsNumber: 'DEVOPS-13457', createdAt: '2024-12-01T11:00:00Z', updatedAt: '2024-12-01T11:00:00Z' },
  { id: '4', releaseId: '1', title: 'Support for multi-file import', description: 'Users can now select and import multiple files simultaneously with a combined import report.', changeType: 'new-feature', moduleTags: ['import'], clientId: '3', ticketNumber: 'TICKET-12350', devopsNumber: 'DEVOPS-13460', createdAt: '2024-12-01T12:00:00Z', updatedAt: '2024-12-01T12:00:00Z' },
  { id: '5', releaseId: '1', title: 'Enhanced report filtering options', description: 'Added advanced filtering capabilities to reports including date ranges, multiple tags, and custom field filters.', changeType: 'enhancement', moduleTags: ['reports'], clientId: '1', ticketNumber: 'TICKET-12351', createdAt: '2024-12-01T13:00:00Z', updatedAt: '2024-12-01T13:00:00Z' },
  { id: '6', releaseId: '2', title: 'Enhanced security with two-factor authentication', description: 'Implemented 2FA for all user accounts using TOTP. Supports authenticator apps like Google Authenticator and Authy.', changeType: 'new-feature', moduleTags: ['security'], clientId: '3', ticketNumber: 'TICKET-12347', devopsNumber: 'DEVOPS-13458', createdAt: '2024-11-15T09:00:00Z', updatedAt: '2024-11-15T09:00:00Z' },
  { id: '7', releaseId: '2', title: 'Fixed report generation timeout issues', description: 'Resolved timeout errors when generating large reports with 10,000+ records. Implemented streaming and pagination.', changeType: 'bug-fix', moduleTags: ['reports'], clientId: '1', ticketNumber: 'TICKET-12348', createdAt: '2024-11-15T09:30:00Z', updatedAt: '2024-11-15T09:30:00Z' },
  { id: '8', releaseId: '2', title: 'Password complexity requirements', description: 'Added configurable password complexity rules including minimum length, special characters, and password history.', changeType: 'enhancement', moduleTags: ['security'], ticketNumber: 'TICKET-12352', devopsNumber: 'DEVOPS-13461', createdAt: '2024-11-15T10:00:00Z', updatedAt: '2024-11-15T10:00:00Z' },
  { id: '9', releaseId: '2', title: 'Fixed dashboard widget refresh issue', description: 'Resolved a bug where dashboard widgets would not refresh automatically after data updates.', changeType: 'bug-fix', moduleTags: ['dashboard'], clientId: '2', ticketNumber: 'TICKET-12353', createdAt: '2024-11-15T11:00:00Z', updatedAt: '2024-11-15T11:00:00Z' },
  { id: '10', releaseId: '3', title: 'New publisher workflow automation', description: 'Automated the publishing process with customizable approval workflows. Supports multi-level approvals and email notifications.', changeType: 'new-feature', moduleTags: ['publisher'], clientId: '2', ticketNumber: 'TICKET-12349', devopsNumber: 'DEVOPS-13459', createdAt: '2024-10-20T14:00:00Z', updatedAt: '2024-10-20T14:00:00Z' },
  { id: '11', releaseId: '3', title: 'Content version control', description: 'Added version history tracking for published content. Users can view, compare, and restore previous versions.', changeType: 'new-feature', moduleTags: ['publisher'], ticketNumber: 'TICKET-12354', devopsNumber: 'DEVOPS-13462', createdAt: '2024-10-20T15:00:00Z', updatedAt: '2024-10-20T15:00:00Z' },
  { id: '12', releaseId: '4', title: 'PDF export functionality', description: 'Added ability to export reports and data directly to PDF format with customizable templates and branding options.', changeType: 'new-feature', moduleTags: ['export'], clientId: '1', ticketNumber: 'TICKET-12355', devopsNumber: 'DEVOPS-13463', createdAt: '2024-10-05T10:00:00Z', updatedAt: '2024-10-05T10:00:00Z' },
  { id: '13', releaseId: '4', title: 'Fixed CSV export date formatting', description: 'Resolved issue where dates were exported in inconsistent formats. Now uses ISO 8601 standard by default.', changeType: 'bug-fix', moduleTags: ['export'], clientId: '2', ticketNumber: 'TICKET-12356', createdAt: '2024-10-05T11:00:00Z', updatedAt: '2024-10-05T11:00:00Z' },
  { id: '14', releaseId: '4', title: 'Batch export scheduling', description: 'Users can now schedule automated exports to run at specific times or intervals. Supports email delivery.', changeType: 'new-feature', moduleTags: ['export', 'systems'], ticketNumber: 'TICKET-12357', devopsNumber: 'DEVOPS-13464', createdAt: '2024-10-05T12:00:00Z', updatedAt: '2024-10-05T12:00:00Z' },
  { id: '15', releaseId: '5', title: 'Dynamic pack creation interface', description: 'New UI for creating and managing packs with drag-and-drop functionality. Includes templates for common pack types.', changeType: 'new-feature', moduleTags: ['packs'], clientId: '3', ticketNumber: 'TICKET-12358', devopsNumber: 'DEVOPS-13465', createdAt: '2024-09-20T14:00:00Z', updatedAt: '2024-09-20T14:00:00Z' },
  { id: '16', releaseId: '5', title: 'Pack dependency validation', description: 'Added automatic validation of pack dependencies to prevent circular references and ensure proper installation order.', changeType: 'enhancement', moduleTags: ['packs'], ticketNumber: 'TICKET-12359', createdAt: '2024-09-20T15:00:00Z', updatedAt: '2024-09-20T15:00:00Z' },
  { id: '17', releaseId: '5', title: 'Fixed pack installation failures', description: 'Resolved issues causing pack installation to fail when network connectivity was intermittent. Added retry logic.', changeType: 'bug-fix', moduleTags: ['packs'], clientId: '1', ticketNumber: 'TICKET-12360', createdAt: '2024-09-20T16:00:00Z', updatedAt: '2024-09-20T16:00:00Z' },
  { id: '18', releaseId: '6', title: 'System health monitoring dashboard', description: 'New dashboard for monitoring system health metrics including CPU, memory, disk usage, and API response times.', changeType: 'new-feature', moduleTags: ['systems', 'dashboard'], ticketNumber: 'TICKET-12361', devopsNumber: 'DEVOPS-13466', createdAt: '2024-09-05T10:00:00Z', updatedAt: '2024-09-05T10:00:00Z' },
  { id: '19', releaseId: '6', title: 'Automated backup configuration', description: 'Added automated backup scheduling with configurable retention policies. Supports full and incremental backups.', changeType: 'new-feature', moduleTags: ['systems'], clientId: '2', ticketNumber: 'TICKET-12362', devopsNumber: 'DEVOPS-13467', createdAt: '2024-09-05T11:00:00Z', updatedAt: '2024-09-05T11:00:00Z' },
  { id: '20', releaseId: '6', title: 'Fixed memory leak in background processes', description: 'Resolved critical memory leak affecting long-running background tasks. System now properly releases resources.', changeType: 'bug-fix', moduleTags: ['systems'], ticketNumber: 'TICKET-12363', createdAt: '2024-09-05T12:00:00Z', updatedAt: '2024-09-05T12:00:00Z' },
  { id: '21', releaseId: '7', title: 'Visual report builder', description: 'New drag-and-drop report builder allowing users to create custom reports without SQL knowledge. 20+ templates.', changeType: 'new-feature', moduleTags: ['reports'], clientId: '1', ticketNumber: 'TICKET-12364', devopsNumber: 'DEVOPS-13468', createdAt: '2024-08-20T14:00:00Z', updatedAt: '2024-08-20T14:00:00Z' },
  { id: '22', releaseId: '7', title: 'Scheduled report delivery', description: 'Users can schedule reports to be automatically generated and emailed to recipients on a recurring basis.', changeType: 'new-feature', moduleTags: ['reports'], ticketNumber: 'TICKET-12365', devopsNumber: 'DEVOPS-13469', createdAt: '2024-08-20T15:00:00Z', updatedAt: '2024-08-20T15:00:00Z' },
  { id: '23', releaseId: '7', title: 'Fixed report parameter handling', description: 'Resolved issue where report parameters with special characters were not properly encoded.', changeType: 'bug-fix', moduleTags: ['reports'], clientId: '3', ticketNumber: 'TICKET-12366', createdAt: '2024-08-20T16:00:00Z', updatedAt: '2024-08-20T16:00:00Z' },
  { id: '24', releaseId: '8', title: 'Role-based access control (RBAC) enhancement', description: 'Enhanced RBAC system with fine-grained permissions. Admins can now create custom roles with specific permission sets.', changeType: 'enhancement', moduleTags: ['security'], ticketNumber: 'TICKET-12367', devopsNumber: 'DEVOPS-13470', createdAt: '2024-08-05T10:00:00Z', updatedAt: '2024-08-05T10:00:00Z' },
  { id: '25', releaseId: '8', title: 'Audit log improvements', description: 'Enhanced audit logging to capture all user actions including login attempts, data changes, and configuration updates.', changeType: 'enhancement', moduleTags: ['security'], clientId: '2', ticketNumber: 'TICKET-12368', createdAt: '2024-08-05T11:00:00Z', updatedAt: '2024-08-05T11:00:00Z' },
  { id: '26', releaseId: '8', title: 'Fixed session timeout vulnerability', description: 'Resolved security vulnerability where user sessions were not properly invalidated after configured timeout.', changeType: 'bug-fix', moduleTags: ['security'], ticketNumber: 'TICKET-12369', createdAt: '2024-08-05T12:00:00Z', updatedAt: '2024-08-05T12:00:00Z' },
  { id: '27', releaseId: '9', title: 'Support for JSON and XML imports', description: 'Extended import functionality to support JSON and XML formats in addition to CSV and Excel. Automatic schema detection.', changeType: 'new-feature', moduleTags: ['import'], clientId: '1', ticketNumber: 'TICKET-12370', devopsNumber: 'DEVOPS-13471', createdAt: '2024-07-20T14:00:00Z', updatedAt: '2024-07-20T14:00:00Z' },
  { id: '28', releaseId: '9', title: 'Import validation rules engine', description: 'New validation rules engine allowing admins to define custom validation rules for imported data.', changeType: 'new-feature', moduleTags: ['import'], ticketNumber: 'TICKET-12371', devopsNumber: 'DEVOPS-13472', createdAt: '2024-07-20T15:00:00Z', updatedAt: '2024-07-20T15:00:00Z' },
  { id: '29', releaseId: '9', title: 'Fixed import progress tracking', description: 'Resolved issue where import progress bar would freeze at 99% for large files. Progress now accurate.', changeType: 'bug-fix', moduleTags: ['import'], clientId: '3', ticketNumber: 'TICKET-12372', createdAt: '2024-07-20T16:00:00Z', updatedAt: '2024-07-20T16:00:00Z' },
  { id: '30', releaseId: '10', title: 'Real-time data streaming', description: 'Dashboard widgets now support real-time data streaming using WebSockets. Data updates appear instantly.', changeType: 'new-feature', moduleTags: ['dashboard'], ticketNumber: 'TICKET-12373', devopsNumber: 'DEVOPS-13473', createdAt: '2024-07-05T10:00:00Z', updatedAt: '2024-07-05T10:00:00Z' },
  { id: '31', releaseId: '10', title: 'Custom dashboard themes', description: 'Users can now create and apply custom color themes to their dashboards. Includes light/dark mode support.', changeType: 'new-feature', moduleTags: ['dashboard'], clientId: '2', ticketNumber: 'TICKET-12374', devopsNumber: 'DEVOPS-13474', createdAt: '2024-07-05T11:00:00Z', updatedAt: '2024-07-05T11:00:00Z' },
  { id: '32', releaseId: '10', title: 'Fixed chart rendering on mobile', description: 'Resolved display issues where charts would overflow or render incorrectly on mobile devices.', changeType: 'bug-fix', moduleTags: ['dashboard'], clientId: '1', ticketNumber: 'TICKET-12375', createdAt: '2024-07-05T12:00:00Z', updatedAt: '2024-07-05T12:00:00Z' },
];

const seedReleases: Release[] = [
  { id: '1', version: '2.5.0', releaseDate: '2024-12-01', title: 'Performance & Analytics Update', description: 'Major performance improvements and new analytics features including enhanced import processing, dashboard widgets, and advanced report filtering.', isPublished: true, createdAt: '2024-12-01T10:00:00Z', updatedAt: '2024-12-01T10:00:00Z' },
  { id: '2', version: '2.4.5', releaseDate: '2024-11-15', title: 'Security & Stability Release', description: 'Enhanced security features including two-factor authentication, password complexity requirements, and critical bug fixes.', isPublished: true, createdAt: '2024-11-15T09:00:00Z', updatedAt: '2024-11-15T09:00:00Z' },
  { id: '3', version: '2.4.0', releaseDate: '2024-10-20', title: 'Publisher Workflow Automation', description: 'New automated workflows for content publishing with multi-level approvals and version control.', isPublished: true, createdAt: '2024-10-20T14:00:00Z', updatedAt: '2024-10-20T14:00:00Z' },
  { id: '4', version: '2.3.5', releaseDate: '2024-10-05', title: 'Export Enhancements', description: 'Added PDF export functionality, batch export scheduling, and fixes for CSV date formatting issues.', isPublished: true, createdAt: '2024-10-05T10:00:00Z', updatedAt: '2024-10-05T10:00:00Z' },
  { id: '5', version: '2.3.0', releaseDate: '2024-09-20', title: 'Packs Management Overhaul', description: 'New dynamic pack creation interface with drag-and-drop, dependency validation, and improved installation reliability.', isPublished: true, createdAt: '2024-09-20T14:00:00Z', updatedAt: '2024-09-20T14:00:00Z' },
  { id: '6', version: '2.2.5', releaseDate: '2024-09-05', title: 'System Monitoring & Health', description: 'System health monitoring dashboard, automated backup configuration, and critical memory leak fix.', isPublished: true, createdAt: '2024-09-05T10:00:00Z', updatedAt: '2024-09-05T10:00:00Z' },
  { id: '7', version: '2.2.0', releaseDate: '2024-08-20', title: 'Visual Report Builder', description: 'New drag-and-drop report builder with 20+ templates, scheduled report delivery, and improved parameter handling.', isPublished: true, createdAt: '2024-08-20T14:00:00Z', updatedAt: '2024-08-20T14:00:00Z' },
  { id: '8', version: '2.1.5', releaseDate: '2024-08-05', title: 'Enhanced Security & Audit Logging', description: 'Enhanced RBAC with custom roles, improved audit logging with search, and session timeout vulnerability fix.', isPublished: true, createdAt: '2024-08-05T10:00:00Z', updatedAt: '2024-08-05T10:00:00Z' },
  { id: '9', version: '2.1.0', releaseDate: '2024-07-20', title: 'Import System Overhaul', description: 'Support for JSON and XML imports, validation rules engine, and fixes for import progress tracking.', isPublished: true, createdAt: '2024-07-20T14:00:00Z', updatedAt: '2024-07-20T14:00:00Z' },
  { id: '10', version: '2.0.5', releaseDate: '2024-07-05', title: 'Dashboard Real-time Updates', description: 'Real-time data streaming with WebSockets, custom dashboard themes with light/dark mode, and mobile chart rendering fixes.', isPublished: true, createdAt: '2024-07-05T10:00:00Z', updatedAt: '2024-07-05T10:00:00Z' },
];

const seedClients: Client[] = [
  { id: '1', name: 'Acme Corporation', code: 'ACME', description: 'Enterprise client with multiple locations', isActive: true, createdAt: '2024-01-15T10:00:00Z', updatedAt: '2024-01-15T10:00:00Z' },
  { id: '2', name: 'Global Tech Solutions', code: 'GTS', description: 'Technology services provider', isActive: true, createdAt: '2024-02-20T14:30:00Z', updatedAt: '2024-02-20T14:30:00Z' },
  { id: '3', name: 'Innovation Labs', code: 'INNOVLAB', description: 'Research and development firm', isActive: true, createdAt: '2024-03-10T09:15:00Z', updatedAt: '2024-03-10T09:15:00Z' },
  { id: '4', name: 'Legacy Systems Inc', code: 'LSI', description: 'Former client - no longer active', isActive: false, createdAt: '2023-06-01T08:00:00Z', updatedAt: '2024-01-01T12:00:00Z' },
];

const seedSqlConnections: SqlConnection[] = [
  { id: '1', name: 'Production DB', server: 'prod-sql-01.internal', database: 'WhatsNewProd', useIntegratedSecurity: true, isActive: true, createdAt: '2024-06-01T10:00:00Z' },
  { id: '2', name: 'Staging DB', server: 'staging-sql-01.internal', database: 'WhatsNewStaging', username: 'app_user', useIntegratedSecurity: false, isActive: true, createdAt: '2024-06-15T14:00:00Z' },
];

const seedSqlQueries: SqlQuery[] = [
  { id: '1', connectionId: '1', name: 'Get Recent Changes', description: 'Fetches changes from the last 30 days', queryText: 'SELECT TOP 100 * FROM Changes WHERE CreatedAt >= DATEADD(day, -30, GETDATE()) ORDER BY CreatedAt DESC', isActive: true, lastExecuted: '2024-11-28T15:30:00Z', createdAt: '2024-07-01T10:00:00Z' },
  { id: '2', connectionId: '1', name: 'Release Summary Report', description: 'Summary statistics per release', queryText: 'SELECT r.Version, r.ReleaseDate, COUNT(c.Id) as ChangeCount FROM Releases r LEFT JOIN Changes c ON r.Id = c.ReleaseId GROUP BY r.Version, r.ReleaseDate ORDER BY r.ReleaseDate DESC', isActive: true, createdAt: '2024-07-15T11:00:00Z' },
];

// ─── Mutable State ─────────────────────────────────────
let users: User[] = [...seedUsers];
let currentUser: PublicUser | null = hydrate<PublicUser | null>('auth', null);
let releases: Release[] = hydrate<Release[]>('releases', seedReleases);
let changes: Change[] = hydrate<Change[]>('changes', seedChanges);
let tags: Tag[] = hydrate<Tag[]>('tags', seedTags);
let clients: Client[] = hydrate<Client[]>('clients', seedClients);
let sqlConnections: SqlConnection[] = hydrate<SqlConnection[]>('connections', seedSqlConnections);
let sqlQueries: SqlQuery[] = hydrate<SqlQuery[]>('queries', seedSqlQueries);

// ─── Auth Methods ──────────────────────────────────────
function login(username: string, password: string): PublicUser | null {
  const user = users.find(u => u.username === username && u.password === password);
  if (!user) return null;
  currentUser = { id: user.id, name: user.name, role: user.role };
  notify('auth');
  return currentUser;
}

function loginAs(publicUser: PublicUser): PublicUser {
  currentUser = { ...publicUser };
  notify('auth');
  return currentUser;
}

function logout(): void {
  currentUser = null;
  notify('auth');
}

function getCurrentUser(): PublicUser | null {
  return currentUser;
}

function getUsers(): PublicUser[] {
  return users.map(u => ({ id: u.id, name: u.name, role: u.role }));
}

function getUserById(id: string): User | undefined {
  return users.find(u => u.id === id);
}

// ─── Release Methods ───────────────────────────────────
function getAllReleases(): Release[] {
  return releases;
}

function getReleasesWithChanges(): ReleaseWithChanges[] {
  return releases.map(r => ({
    ...r,
    changes: changes.filter(c => c.releaseId === r.id),
  }));
}

function getReleaseById(id: string): Release | undefined {
  return releases.find(r => r.id === id);
}

function createRelease(data: Omit<Release, 'id' | 'createdAt' | 'updatedAt'>): Release {
  const release: Release = {
    id: generateId(),
    ...data,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  };
  releases = [release, ...releases];
  notify('releases');
  return release;
}

function updateRelease(id: string, data: Partial<Release>): Release | null {
  const idx = releases.findIndex(r => r.id === id);
  if (idx === -1) return null;
  releases[idx] = { ...releases[idx], ...data, updatedAt: new Date().toISOString() };
  releases = [...releases];
  notify('releases');
  return releases[idx];
}

function deleteRelease(id: string): boolean {
  const before = releases.length;
  releases = releases.filter(r => r.id !== id);
  if (releases.length < before) {
    // Cross-domain: delete related changes
    changes = changes.filter(c => c.releaseId !== id);
    notify('releases');
    notify('changes');
    return true;
  }
  return false;
}

// ─── Change Methods ────────────────────────────────────
function getAllChanges(): Change[] {
  return changes;
}

function getChangesByReleaseId(releaseId: string): Change[] {
  return changes.filter(c => c.releaseId === releaseId);
}

function getChangeById(id: string): Change | undefined {
  return changes.find(c => c.id === id);
}

function createChange(data: Omit<Change, 'id' | 'createdAt' | 'updatedAt'>): Change {
  const change: Change = {
    id: generateId(),
    ...data,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  };
  changes = [...changes, change];
  notify('changes');
  return change;
}

function updateChange(id: string, data: Partial<Change>): Change | null {
  const idx = changes.findIndex(c => c.id === id);
  if (idx === -1) return null;
  changes[idx] = { ...changes[idx], ...data, updatedAt: new Date().toISOString() };
  changes = [...changes];
  notify('changes');
  return changes[idx];
}

function deleteChange(id: string): boolean {
  const before = changes.length;
  changes = changes.filter(c => c.id !== id);
  if (changes.length < before) {
    notify('changes');
    return true;
  }
  return false;
}

// ─── Tag Methods ───────────────────────────────────────
function getAllTags(): Tag[] {
  return tags;
}

function getTagsByType(type: string): Tag[] {
  return tags.filter(t => t.type === type);
}

function getTagById(id: string): Tag | undefined {
  return tags.find(t => t.id === id);
}

function createTag(data: Omit<Tag, 'id'>): Tag {
  const tag: Tag = { id: generateId(), ...data };
  tags = [...tags, tag];
  notify('tags');
  return tag;
}

function updateTag(id: string, data: Partial<Tag>): Tag | null {
  const idx = tags.findIndex(t => t.id === id);
  if (idx === -1) return null;
  tags[idx] = { ...tags[idx], ...data };
  tags = [...tags];
  notify('tags');
  return tags[idx];
}

function deleteTag(id: string): boolean {
  const before = tags.length;
  tags = tags.filter(t => t.id !== id);
  if (tags.length < before) {
    notify('tags');
    return true;
  }
  return false;
}

// ─── Client Methods ────────────────────────────────────
function getAllClients(): Client[] {
  return clients;
}

function getClientById(id: string): Client | undefined {
  return clients.find(c => c.id === id);
}

function createClient(data: Omit<Client, 'id' | 'createdAt' | 'updatedAt'>): Client {
  const client: Client = {
    id: generateId(),
    ...data,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  };
  clients = [...clients, client];
  notify('clients');
  return client;
}

function updateClient(id: string, data: Partial<Client>): Client | null {
  const idx = clients.findIndex(c => c.id === id);
  if (idx === -1) return null;
  clients[idx] = { ...clients[idx], ...data, updatedAt: new Date().toISOString() };
  clients = [...clients];
  notify('clients');
  return clients[idx];
}

function deleteClient(id: string): boolean {
  const before = clients.length;
  clients = clients.filter(c => c.id !== id);
  if (clients.length < before) {
    // Cross-domain: clear clientId from changes
    changes = changes.map(ch => ch.clientId === id ? { ...ch, clientId: undefined } : ch);
    notify('clients');
    notify('changes');
    return true;
  }
  return false;
}

// ─── SQL Connection Methods ────────────────────────────
function getAllConnections(): SqlConnection[] {
  return sqlConnections;
}

function getConnectionById(id: string): SqlConnection | undefined {
  return sqlConnections.find(c => c.id === id);
}

function createConnection(data: Omit<SqlConnection, 'id' | 'createdAt'>): SqlConnection {
  const conn: SqlConnection = {
    id: generateId(),
    ...data,
    createdAt: new Date().toISOString(),
  };
  sqlConnections = [...sqlConnections, conn];
  notify('connections');
  return conn;
}

function updateConnection(id: string, data: Partial<SqlConnection>): SqlConnection | null {
  const idx = sqlConnections.findIndex(c => c.id === id);
  if (idx === -1) return null;
  sqlConnections[idx] = { ...sqlConnections[idx], ...data };
  sqlConnections = [...sqlConnections];
  notify('connections');
  return sqlConnections[idx];
}

function deleteConnection(id: string): boolean {
  const before = sqlConnections.length;
  sqlConnections = sqlConnections.filter(c => c.id !== id);
  if (sqlConnections.length < before) {
    // Cross-domain: delete related queries
    sqlQueries = sqlQueries.filter(q => q.connectionId !== id);
    notify('connections');
    notify('queries');
    return true;
  }
  return false;
}

// ─── SQL Query Methods ─────────────────────────────────
function getAllQueries(): SqlQuery[] {
  return sqlQueries;
}

function getQueryById(id: string): SqlQuery | undefined {
  return sqlQueries.find(q => q.id === id);
}

function createQuery(data: Omit<SqlQuery, 'id' | 'createdAt'>): SqlQuery {
  const query: SqlQuery = {
    id: generateId(),
    ...data,
    createdAt: new Date().toISOString(),
  };
  sqlQueries = [...sqlQueries, query];
  notify('queries');
  return query;
}

function updateQuery(id: string, data: Partial<SqlQuery>): SqlQuery | null {
  const idx = sqlQueries.findIndex(q => q.id === id);
  if (idx === -1) return null;
  sqlQueries[idx] = { ...sqlQueries[idx], ...data };
  sqlQueries = [...sqlQueries];
  notify('queries');
  return sqlQueries[idx];
}

function deleteQuery(id: string): boolean {
  const before = sqlQueries.length;
  sqlQueries = sqlQueries.filter(q => q.id !== id);
  if (sqlQueries.length < before) {
    notify('queries');
    return true;
  }
  return false;
}

// ─── Computed Getters ──────────────────────────────────
function getReleasesFiltered(filters: {
  searchTerm?: string;
  changeType?: string;
  moduleTag?: string;
  fromDate?: string;
  toDate?: string;
}): ReleaseWithChanges[] {
  let result = getReleasesWithChanges();

  if (filters.searchTerm) {
    const term = filters.searchTerm.toLowerCase();
    result = result.filter(r => {
      const matchesVersion = r.version.toLowerCase().includes(term);
      const matchesTitle = r.title?.toLowerCase().includes(term);
      const matchesDesc = r.description?.toLowerCase().includes(term);
      const matchesChanges = r.changes.some(c =>
        c.title.toLowerCase().includes(term) || c.description.toLowerCase().includes(term)
      );
      return matchesVersion || matchesTitle || matchesDesc || matchesChanges;
    });
  }

  if (filters.changeType && filters.changeType !== 'all') {
    result = result.map(r => ({
      ...r,
      changes: r.changes.filter(c => c.changeType === filters.changeType),
    })).filter(r => r.changes.length > 0);
  }

  if (filters.moduleTag && filters.moduleTag !== 'all') {
    const tagObj = tags.find(t => t.id === filters.moduleTag);
    const tagValue = tagObj?.value || filters.moduleTag;
    result = result.map(r => ({
      ...r,
      changes: r.changes.filter(c => c.moduleTags.includes(tagValue)),
    })).filter(r => r.changes.length > 0);
  }

  if (filters.fromDate) {
    result = result.filter(r => r.releaseDate >= filters.fromDate!);
  }
  if (filters.toDate) {
    result = result.filter(r => r.releaseDate <= filters.toDate!);
  }

  return result;
}

function getStatistics() {
  return {
    totalReleases: releases.length,
    totalChanges: changes.length,
    bugFixCount: changes.filter(c => c.changeType === 'bug-fix').length,
    newFeatureCount: changes.filter(c => c.changeType === 'new-feature').length,
    enhancementCount: changes.filter(c => c.changeType === 'enhancement').length,
    publishedReleases: releases.filter(r => r.isPublished).length,
    firstReleaseDate: releases.length > 0 ? releases[releases.length - 1].releaseDate : undefined,
    latestReleaseDate: releases.length > 0 ? releases[0].releaseDate : undefined,
  };
}

function getDashboardSummary(): DashboardSummary {
  const published = releases.filter(r => r.isPublished).length;
  const moduleTags = tags.filter(t => t.type === 'module');
  const latestRelease = releases[0];
  const thisMonth = new Date().toISOString().slice(0, 7);
  const releasesThisMonth = releases.filter(r => r.releaseDate.slice(0, 7) === thisMonth).length;
  const changesThisMonth = changes.filter(c => c.createdAt.slice(0, 7) === thisMonth).length;

  return {
    totalReleases: releases.length,
    totalChanges: changes.length,
    publishedReleases: published,
    averageChangesPerRelease: releases.length > 0 ? parseFloat((changes.length / releases.length).toFixed(1)) : 0,
    lastReleaseDate: latestRelease?.releaseDate || '',
    upcomingReleases: releases.filter(r => !r.isPublished).length,
    totalModules: moduleTags.length,
    releasesThisMonth,
    changesThisMonth,
    latestReleaseDate: latestRelease?.releaseDate || '',
    latestVersion: latestRelease?.version || '',
    clientRequestCount: changes.filter(c => c.clientId).length,
    ticketCount: changes.filter(c => c.ticketNumber).length,
  };
}

function getTimelineData(): TimelineEntry[] {
  const monthMap = new Map<string, TimelineEntry>();
  releases.forEach(r => {
    const d = new Date(r.releaseDate);
    const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`;
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    if (!monthMap.has(key)) {
      monthMap.set(key, {
        month: `${monthNames[d.getMonth()]} ${d.getFullYear()}`,
        monthName: monthNames[d.getMonth()],
        year: d.getFullYear(),
        releases: 0, changes: 0, releaseCount: 0, totalChanges: 0,
        bugFixes: 0, newFeatures: 0, enhancements: 0,
      });
    }
    const entry = monthMap.get(key)!;
    entry.releases++;
    entry.releaseCount++;
    const releaseChanges = changes.filter(c => c.releaseId === r.id);
    entry.changes += releaseChanges.length;
    entry.totalChanges += releaseChanges.length;
    entry.bugFixes += releaseChanges.filter(c => c.changeType === 'bug-fix').length;
    entry.newFeatures += releaseChanges.filter(c => c.changeType === 'new-feature').length;
    entry.enhancements += releaseChanges.filter(c => c.changeType === 'enhancement').length;
  });
  return Array.from(monthMap.entries())
    .sort(([a], [b]) => a.localeCompare(b))
    .map(([, v]) => v);
}

function getModuleDistribution(): ModuleDistribution[] {
  const moduleTags2 = tags.filter(t => t.type === 'module');
  const total = changes.length || 1;
  return moduleTags2.map(tag => {
    const matching = changes.filter(c => c.moduleTags.includes(tag.value));
    return {
      tagId: tag.id,
      moduleName: tag.label,
      moduleValue: tag.value,
      changeCount: matching.length,
      bugFixes: matching.filter(c => c.changeType === 'bug-fix').length,
      newFeatures: matching.filter(c => c.changeType === 'new-feature').length,
      enhancements: matching.filter(c => c.changeType === 'enhancement').length,
      count: matching.length,
      percentage: Math.round((matching.length / total) * 100),
    };
  }).sort((a, b) => b.changeCount - a.changeCount);
}

function getChangeTypeDistribution(): ChangeTypeDistribution[] {
  const types = [
    { key: 'bug-fix', label: 'Bug Fix' },
    { key: 'new-feature', label: 'New Feature' },
    { key: 'enhancement', label: 'Enhancement' },
  ];
  const total = changes.length || 1;
  return types.map(t => {
    const count = changes.filter(c => c.changeType === t.key).length;
    return {
      tagId: t.key,
      moduleName: t.label,
      moduleValue: t.key,
      changeCount: count,
      count,
      percentage: Math.round((count / total) * 100),
    };
  });
}

function getClientDistribution(): ClientDistribution[] {
  const result: ClientDistribution[] = [];
  const clientIds = new Set(changes.map(c => c.clientId).filter(Boolean));
  clientIds.forEach(cid => {
    const client = clients.find(c => c.id === cid);
    const matching = changes.filter(c => c.clientId === cid);
    result.push({
      clientId: cid || null,
      clientName: client?.name || 'Unknown',
      clientCode: client?.code || 'UNK',
      changeCount: matching.length,
      bugFixes: matching.filter(c => c.changeType === 'bug-fix').length,
      newFeatures: matching.filter(c => c.changeType === 'new-feature').length,
      enhancements: matching.filter(c => c.changeType === 'enhancement').length,
      count: matching.length,
      percentage: 0,
    });
  });
  const noClient = changes.filter(c => !c.clientId);
  if (noClient.length > 0) {
    result.push({
      clientId: null,
      clientName: 'Internal',
      clientCode: 'INT',
      changeCount: noClient.length,
      bugFixes: noClient.filter(c => c.changeType === 'bug-fix').length,
      newFeatures: noClient.filter(c => c.changeType === 'new-feature').length,
      enhancements: noClient.filter(c => c.changeType === 'enhancement').length,
      count: noClient.length,
      percentage: 0,
    });
  }
  const total = changes.length || 1;
  result.forEach(r => { r.percentage = Math.round((r.count / total) * 100); });
  return result.sort((a, b) => b.changeCount - a.changeCount);
}

function getRecentActivity(topN: number = 20): RecentActivity[] {
  const activities: RecentActivity[] = [];
  releases.forEach(r => {
    activities.push({
      entityId: r.id, entityName: r.version,
      activityType: 'Release', activityDate: r.releaseDate,
      description: r.title,
    });
  });
  changes.forEach(c => {
    activities.push({
      entityId: c.id, entityName: c.title,
      activityType: 'Change', activityDate: c.createdAt,
      description: c.changeType,
    });
  });
  return activities
    .sort((a, b) => new Date(b.activityDate).getTime() - new Date(a.activityDate).getTime())
    .slice(0, topN);
}

function getReleaseVelocity(): ReleaseVelocity {
  return { averageTimeBetweenReleases: 15, releasesPerMonth: 2, trend: 'stable' };
}

function getTopReleases(topN: number = 5): TopRelease[] {
  return releases.slice(0, topN).map(r => ({
    releaseId: r.id, version: r.version,
    changeCount: changes.filter(c => c.releaseId === r.id).length,
    releaseDate: r.releaseDate,
  }));
}

function getTimeToActionMetrics(): TimeToActionMetrics {
  return {
    byChangeType: [
      { changeType: 'bug-fix', label: 'Bug Fix', averageTotalTime: 8.5, averageDevTime: 2.5, averageTestTime: 1.5, averageReleaseTime: 4.5, submittedToDeveloped: 2.5, developedToTested: 1.5, testedToReleased: 4.5, count: changes.filter(c => c.changeType === 'bug-fix').length },
      { changeType: 'enhancement', label: 'Enhancement', averageTotalTime: 12.3, averageDevTime: 4.2, averageTestTime: 2.1, averageReleaseTime: 6.0, submittedToDeveloped: 4.2, developedToTested: 2.1, testedToReleased: 6.0, count: changes.filter(c => c.changeType === 'enhancement').length },
      { changeType: 'new-feature', label: 'New Feature', averageTotalTime: 18.7, averageDevTime: 8.5, averageTestTime: 3.2, averageReleaseTime: 7.0, submittedToDeveloped: 8.5, developedToTested: 3.2, testedToReleased: 7.0, count: changes.filter(c => c.changeType === 'new-feature').length },
    ],
    timeline: [
      { month: 'Jul', monthName: 'Jul', bugFix: 8.5, enhancement: 12.5, newFeature: 18.5 },
      { month: 'Aug', monthName: 'Aug', bugFix: 8.3, enhancement: 12.2, newFeature: 18.2 },
      { month: 'Sep', monthName: 'Sep', bugFix: 8.4, enhancement: 12.3, newFeature: 18.7 },
      { month: 'Oct', monthName: 'Oct', bugFix: 8.5, enhancement: 12.3, newFeature: 18.7 },
      { month: 'Nov', monthName: 'Nov', bugFix: 8.8, enhancement: 12.8, newFeature: 19.2 },
      { month: 'Dec', monthName: 'Dec', bugFix: 8.5, enhancement: 12.5, newFeature: 18.5 },
    ],
    overall: { averageTotalTime: 13.8, fastestCompletion: 3.5, slowestCompletion: 45.2, medianTime: 11.5 },
  };
}

function getPopularTags(topN: number = 10) {
  const tagCounts: Record<string, number> = {};
  changes.forEach(c => {
    c.moduleTags.forEach(t => { tagCounts[t] = (tagCounts[t] || 0) + 1; });
  });
  return Object.entries(tagCounts)
    .map(([tagValue, count]) => {
      const tagObj = tags.find(t => t.value === tagValue);
      return { tag: tagObj?.label || tagValue, count };
    })
    .sort((a, b) => b.count - a.count)
    .slice(0, topN);
}

function searchChanges(query: string): Change[] {
  const term = query.toLowerCase();
  return changes.filter(c =>
    c.title.toLowerCase().includes(term) || c.description.toLowerCase().includes(term)
  );
}

// ─── Reset to Defaults ─────────────────────────────────
function resetToDefaults(): void {
  clearAllPersisted();
  currentUser = null;
  releases = [...seedReleases];
  changes = [...seedChanges];
  tags = [...seedTags];
  clients = [...seedClients];
  sqlConnections = [...seedSqlConnections];
  sqlQueries = [...seedSqlQueries];
  idCounter = 100;
  // Notify all slices
  (Object.keys(subscribers) as Slice[]).forEach(slice => {
    subscribers[slice].forEach(fn => fn());
  });
}

// ─── Public API ────────────────────────────────────────
export const appStore = {
  // Auth
  get currentUser() { return currentUser; },
  get users() { return users; },
  login, loginAs, logout, getCurrentUser, getUsers, getUserById,

  // Releases
  get releases() { return releases; },
  getAllReleases, getReleasesWithChanges, getReleaseById,
  createRelease, updateRelease, deleteRelease,

  // Changes
  get changes() { return changes; },
  getAllChanges, getChangesByReleaseId, getChangeById,
  createChange, updateChange, deleteChange,

  // Tags
  get tags() { return tags; },
  getAllTags, getTagsByType, getTagById,
  createTag, updateTag, deleteTag,

  // Clients
  get clients() { return clients; },
  getAllClients, getClientById,
  createClient, updateClient, deleteClient,

  // SQL Connections
  get sqlConnections() { return sqlConnections; },
  getAllConnections, getConnectionById,
  createConnection, updateConnection, deleteConnection,

  // SQL Queries
  get sqlQueries() { return sqlQueries; },
  getAllQueries, getQueryById,
  createQuery, updateQuery, deleteQuery,

  // Computed
  getReleasesFiltered, getStatistics, getDashboardSummary,
  getTimelineData, getModuleDistribution, getChangeTypeDistribution,
  getClientDistribution, getRecentActivity, getReleaseVelocity,
  getTopReleases, getTimeToActionMetrics, getPopularTags, searchChanges,

  // Reset
  resetToDefaults,

  // Pub/sub
  subscribe(slice: Slice, listener: Listener): () => void {
    subscribers[slice].add(listener);
    return () => { subscribers[slice].delete(listener); };
  },
};
