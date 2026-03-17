import { useMemo } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { BarChart3, TrendingUp, Package, Bug, Sparkles, Zap, Users, Clock, Ticket } from 'lucide-react';
import { Card } from './ui/card';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line } from 'recharts';

const COLORS = ['#456E92', '#5F966C', '#CEA569', '#AB5A5C', '#7AA2C0', '#ec4899', '#06b6d4', '#84cc16'];

export function AnalyticsDashboard() {
  const { reads } = useAppStore('releases', 'changes', 'tags', 'clients');

  const summary = reads.getDashboardSummary();
  const timeline = reads.getTimelineData();
  const moduleDistribution = reads.getModuleDistribution();
  const changeTypeDistribution = reads.getChangeTypeDistribution();
  const clientDistribution = reads.getClientDistribution();
  const topReleases = reads.getTopReleases(5);
  const recentActivity = reads.getRecentActivity(10);
  const timeToAction = reads.getTimeToActionMetrics();

  const pieData = useMemo(() =>
    changeTypeDistribution.map(d => ({
      name: d.moduleName,
      value: d.count,
    })), [changeTypeDistribution]);

  return (
    <div className="space-y-6">
      <div>
        <div className="flex items-center gap-3">
          <div className="p-3 bg-brand-primary-light rounded-lg">
            <BarChart3 className="w-6 h-6 text-brand-primary" />
          </div>
          <div>
            <h1 className="text-3xl text-foreground">Analytics Dashboard</h1>
            <p className="text-muted-foreground">Insights across all releases and changes</p>
          </div>
        </div>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-6">
        <Card className="p-0">
          <div className="p-6 flex items-center gap-4">
            <div className="p-3 bg-brand-primary-light rounded-lg">
              <Package className="w-6 h-6 text-brand-primary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Releases</p>
              <p className="text-2xl font-bold text-foreground">{summary.totalReleases}</p>
            </div>
          </div>
        </Card>
        <Card className="p-0">
          <div className="p-6 flex items-center gap-4">
            <div className="p-3 bg-brand-secondary-light rounded-lg">
              <TrendingUp className="w-6 h-6 text-brand-secondary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Changes</p>
              <p className="text-2xl font-bold text-foreground">{summary.totalChanges}</p>
            </div>
          </div>
        </Card>
        <Card className="p-0 border-l-4 border-l-brand-success">
          <div className="p-6">
            <div className="flex items-center gap-2 text-brand-success text-sm mb-1"><Sparkles className="w-4 h-4" />Features</div>
            <div className="text-2xl font-bold text-foreground">{changeTypeDistribution.find(d => d.moduleValue === 'new-feature')?.count || 0}</div>
          </div>
        </Card>
        <Card className="p-0 border-l-4 border-l-brand-primary">
          <div className="p-6">
            <div className="flex items-center gap-2 text-brand-primary text-sm mb-1"><Zap className="w-4 h-4" />Enhancements</div>
            <div className="text-2xl font-bold text-foreground">{changeTypeDistribution.find(d => d.moduleValue === 'enhancement')?.count || 0}</div>
          </div>
        </Card>
        <Card className="p-0 border-l-4 border-l-brand-error">
          <div className="p-6">
            <div className="flex items-center gap-2 text-brand-error text-sm mb-1"><Bug className="w-4 h-4" />Bug Fixes</div>
            <div className="text-2xl font-bold text-foreground">{changeTypeDistribution.find(d => d.moduleValue === 'bug-fix')?.count || 0}</div>
          </div>
        </Card>
        <Card className="p-0">
          <div className="p-6 flex items-center gap-4">
            <div className="p-3 bg-brand-warning-light rounded-lg">
              <Ticket className="w-6 h-6 text-brand-warning" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Tickets</p>
              <p className="text-2xl font-bold text-foreground">{summary.ticketCount}</p>
            </div>
          </div>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Timeline Chart */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Release Timeline</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={timeline}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="monthName" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="newFeatures" fill="#5F966C" name="Features" />
                <Bar dataKey="enhancements" fill="#456E92" name="Enhancements" />
                <Bar dataKey="bugFixes" fill="#AB5A5C" name="Bug Fixes" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>

        {/* Change Type Distribution Pie */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Change Type Distribution</h3>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie data={pieData} cx="50%" cy="50%" labelLine={false} label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`} outerRadius={100} dataKey="value">
                  {pieData.map((_, i) => (
                    <Cell key={`cell-${i}`} fill={['#5F966C', '#456E92', '#AB5A5C'][i] || COLORS[i]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Module Distribution */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Changes by Module</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={moduleDistribution} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" />
                <YAxis type="category" dataKey="moduleName" width={80} />
                <Tooltip />
                <Bar dataKey="changeCount" fill="#456E92" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>

        {/* Client Distribution */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Changes by Client</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={clientDistribution}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="clientName" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="newFeatures" fill="#5F966C" name="Features" stackId="a" />
                <Bar dataKey="enhancements" fill="#456E92" name="Enhancements" stackId="a" />
                <Bar dataKey="bugFixes" fill="#AB5A5C" name="Bug Fixes" stackId="a" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Time to Action */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Average Time to Release (days)</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={timeToAction.timeline}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="monthName" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="bugFix" stroke="#AB5A5C" name="Bug Fix" />
                <Line type="monotone" dataKey="enhancement" stroke="#456E92" name="Enhancement" />
                <Line type="monotone" dataKey="newFeature" stroke="#5F966C" name="Feature" />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </Card>

        {/* Top Releases */}
        <Card>
          <div className="p-6">
            <h3 className="text-foreground mb-4">Top Releases by Change Count</h3>
            <div className="space-y-3">
              {topReleases.map((r, i) => (
                <div key={r.releaseId} className="flex items-center justify-between p-3 bg-muted/50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <span className="w-6 h-6 bg-brand-primary-light text-brand-primary rounded-full flex items-center justify-center text-xs">{i + 1}</span>
                    <div>
                      <div className="text-foreground text-sm">v{r.version}</div>
                      <div className="text-muted-foreground text-xs">{new Date(r.releaseDate).toLocaleDateString()}</div>
                    </div>
                  </div>
                  <span className="text-sm text-foreground">{r.changeCount} changes</span>
                </div>
              ))}
            </div>
          </div>
        </Card>
      </div>

      {/* Recent Activity */}
      <Card>
        <div className="p-6">
          <h3 className="text-foreground mb-4">Recent Activity</h3>
          <div className="space-y-2">
            {recentActivity.map((act, i) => (
              <div key={`${act.entityId}-${i}`} className="flex items-center gap-3 p-2 hover:bg-muted/50 rounded transition-colors">
                <span className={`px-2 py-0.5 text-xs rounded ${act.activityType === 'Release' ? 'bg-brand-primary-light text-brand-primary' : 'bg-secondary text-secondary-foreground'}`}>{act.activityType}</span>
                <span className="text-sm text-foreground flex-1">{act.entityName}</span>
                <span className="text-xs text-muted-foreground">{new Date(act.activityDate).toLocaleDateString()}</span>
              </div>
            ))}
          </div>
        </div>
      </Card>
    </div>
  );
}