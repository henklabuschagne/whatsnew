import { useState, useEffect, useMemo } from 'react';
import { appStore, type Slice } from '../lib/appStore';
import { api } from '../lib/api';

export function useAppStore(...subscribeTo: Slice[]) {
  const [, bump] = useState(0);

  useEffect(() => {
    const unsubscribes = subscribeTo.map(slice =>
      appStore.subscribe(slice, () => bump(v => v + 1))
    );
    return () => unsubscribes.forEach(unsub => unsub());
    // subscribeTo is static per component - eslint-disable-next-line
  }, [subscribeTo.join(',')]);

  // ─── Reactive State ──────────────────────────────────
  const currentUser = appStore.currentUser;
  const releases = appStore.releases;
  const changes = appStore.changes;
  const tags = appStore.tags;
  const clients = appStore.clients;
  const sqlConnections = appStore.sqlConnections;
  const sqlQueries = appStore.sqlQueries;

  // ─── Sync Read Helpers ───────────────────────────────
  const reads = useMemo(() => ({
    getReleasesWithChanges: () => appStore.getReleasesWithChanges(),
    getReleasesFiltered: (filters: Parameters<typeof appStore.getReleasesFiltered>[0]) =>
      appStore.getReleasesFiltered(filters),
    getReleaseById: (id: string) => appStore.getReleaseById(id),
    getChangesByReleaseId: (releaseId: string) => appStore.getChangesByReleaseId(releaseId),
    getChangeById: (id: string) => appStore.getChangeById(id),
    getTagsByType: (type: string) => appStore.getTagsByType(type),
    getTagById: (id: string) => appStore.getTagById(id),
    getClientById: (id: string) => appStore.getClientById(id),
    getConnectionById: (id: string) => appStore.getConnectionById(id),
    getQueryById: (id: string) => appStore.getQueryById(id),
    getStatistics: () => appStore.getStatistics(),
    getDashboardSummary: () => appStore.getDashboardSummary(),
    getTimelineData: () => appStore.getTimelineData(),
    getModuleDistribution: () => appStore.getModuleDistribution(),
    getChangeTypeDistribution: () => appStore.getChangeTypeDistribution(),
    getClientDistribution: () => appStore.getClientDistribution(),
    getRecentActivity: (topN?: number) => appStore.getRecentActivity(topN),
    getReleaseVelocity: () => appStore.getReleaseVelocity(),
    getTopReleases: (topN?: number) => appStore.getTopReleases(topN),
    getTimeToActionMetrics: () => appStore.getTimeToActionMetrics(),
    getPopularTags: (topN?: number) => appStore.getPopularTags(topN),
    searchChanges: (q: string) => appStore.searchChanges(q),
    getUsers: () => appStore.getUsers(),
    getUserById: (id: string) => appStore.getUserById(id),
  }), []);

  // ─── Async Actions (routed through API layer) ───────
  const actions = useMemo(() => ({
    // Auth
    login: (username: string, password: string) => api.auth.login(username, password),
    loginAs: (user: Parameters<typeof api.auth.loginAs>[0]) => api.auth.loginAs(user),
    logout: () => api.auth.logout(),

    // Releases
    createRelease: (data: Parameters<typeof api.releases.createRelease>[0]) => api.releases.createRelease(data),
    updateRelease: (id: string, data: Parameters<typeof api.releases.updateRelease>[1]) => api.releases.updateRelease(id, data),
    deleteRelease: (id: string) => api.releases.deleteRelease(id),

    // Changes
    createChange: (data: Parameters<typeof api.changes.createChange>[0]) => api.changes.createChange(data),
    updateChange: (id: string, data: Parameters<typeof api.changes.updateChange>[1]) => api.changes.updateChange(id, data),
    deleteChange: (id: string) => api.changes.deleteChange(id),

    // Tags
    createTag: (data: Parameters<typeof api.tags.createTag>[0]) => api.tags.createTag(data),
    updateTag: (id: string, data: Parameters<typeof api.tags.updateTag>[1]) => api.tags.updateTag(id, data),
    deleteTag: (id: string) => api.tags.deleteTag(id),

    // Clients
    createClient: (data: Parameters<typeof api.clients.createClient>[0]) => api.clients.createClient(data),
    updateClient: (id: string, data: Parameters<typeof api.clients.updateClient>[1]) => api.clients.updateClient(id, data),
    deleteClient: (id: string) => api.clients.deleteClient(id),

    // Integrations
    createConnection: (data: Parameters<typeof api.integrations.createConnection>[0]) => api.integrations.createConnection(data),
    updateConnection: (id: string, data: Parameters<typeof api.integrations.updateConnection>[1]) => api.integrations.updateConnection(id, data),
    deleteConnection: (id: string) => api.integrations.deleteConnection(id),
    testConnection: (data: Parameters<typeof api.integrations.testConnection>[0]) => api.integrations.testConnection(data),
    createQuery: (data: Parameters<typeof api.integrations.createQuery>[0]) => api.integrations.createQuery(data),
    updateQuery: (id: string, data: Parameters<typeof api.integrations.updateQuery>[1]) => api.integrations.updateQuery(id, data),
    deleteQuery: (id: string) => api.integrations.deleteQuery(id),
    executeQuery: (id: string) => api.integrations.executeQuery(id),

    // Import/Export
    importExcel: (file: File) => api.importExport.importExcel(file),
    exportExcel: () => api.importExport.exportExcel(),
    downloadTemplate: () => api.importExport.downloadTemplate(),

    // Reset
    resetToDefaults: () => { appStore.resetToDefaults(); },
  }), []);

  return {
    // Reactive state
    currentUser,
    releases,
    changes,
    tags,
    clients,
    sqlConnections,
    sqlQueries,
    // Sync reads
    reads,
    // Async writes
    actions,
  };
}
