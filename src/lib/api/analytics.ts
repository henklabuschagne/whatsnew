import { appStore } from '../appStore';
import { mockApiCall } from './config';
import type { ApiResult } from './types';
import type {
  DashboardSummary, TimelineEntry, ModuleDistribution,
  ChangeTypeDistribution, ClientDistribution, TimeToActionMetrics,
  RecentActivity, ReleaseVelocity, TopRelease,
} from '../../types';

export async function getDashboardSummary(): Promise<ApiResult<DashboardSummary>> {
  return mockApiCall(() => appStore.getDashboardSummary());
}

export async function getTimeline(months?: number): Promise<ApiResult<TimelineEntry[]>> {
  return mockApiCall(() => {
    const data = appStore.getTimelineData();
    return months ? data.slice(-months) : data;
  });
}

export async function getModuleDistribution(): Promise<ApiResult<ModuleDistribution[]>> {
  return mockApiCall(() => appStore.getModuleDistribution());
}

export async function getChangeTypeDistribution(): Promise<ApiResult<ChangeTypeDistribution[]>> {
  return mockApiCall(() => appStore.getChangeTypeDistribution());
}

export async function getClientDistribution(): Promise<ApiResult<ClientDistribution[]>> {
  return mockApiCall(() => appStore.getClientDistribution());
}

export async function getRecentActivity(topN?: number): Promise<ApiResult<RecentActivity[]>> {
  return mockApiCall(() => appStore.getRecentActivity(topN));
}

export async function getReleaseVelocity(): Promise<ApiResult<ReleaseVelocity>> {
  return mockApiCall(() => appStore.getReleaseVelocity());
}

export async function getTopReleases(topN?: number): Promise<ApiResult<TopRelease[]>> {
  return mockApiCall(() => appStore.getTopReleases(topN));
}

export async function getTimeToActionMetrics(): Promise<ApiResult<TimeToActionMetrics>> {
  return mockApiCall(() => appStore.getTimeToActionMetrics());
}

export async function getChangeTrends(): Promise<ApiResult<{ bugFixTrend: string; newFeatureTrend: string; enhancementTrend: string }>> {
  return mockApiCall(() => ({
    bugFixTrend: 'down', newFeatureTrend: 'up', enhancementTrend: 'stable',
  }));
}
