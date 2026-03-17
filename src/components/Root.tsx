import { Outlet, Link, useLocation } from 'react-router';
import { Package, Settings, Tag, Database, LogOut, User, BarChart3, Users, RotateCcw, FileSpreadsheet, Newspaper } from 'lucide-react';
import { Button } from './ui/button';
import { useAppStore } from '../hooks/useAppStore';
import { apiConfig } from '../lib/api';
import { useState } from 'react';
import { toast } from "sonner@2.0.3";
import { Badge } from './ui/badge';

export function Root() {
  const location = useLocation();
  const { currentUser, actions } = useAppStore('auth');
  const isAdmin = currentUser?.role === 'admin';
  const [showDevPanel, setShowDevPanel] = useState(false);

  const handleLogout = async () => {
    await actions.logout();
    window.location.replace('/');
  };

  const handleReset = () => {
    actions.resetToDefaults();
    toast.success('All data reset to defaults');
    window.location.reload();
  };

  const navLink = (to: string, icon: React.ReactNode, label: string) => {
    const isActive = location.pathname === to;
    return (
      <Link
        to={to}
        className={`flex items-center gap-3 px-4 py-3 rounded-lg text-sm transition-colors ${
          isActive
            ? 'bg-brand-primary-light text-brand-primary font-medium'
            : 'text-foreground/80 hover:bg-muted hover:text-foreground'
        }`}
      >
        <span className={isActive ? 'text-brand-primary' : 'text-muted-foreground'}>{icon}</span>
        <span>{label}</span>
      </Link>
    );
  };

  return (
    <div className="flex h-screen bg-background">
      <a
        href="#main-content"
        className="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 focus:z-50 focus:px-4 focus:py-2 focus:bg-brand-primary focus:text-white focus:rounded-md"
      >
        Skip to main content
      </a>

      {/* Sidebar */}
      <aside className="w-64 bg-white h-screen flex flex-col border-r border-border" role="navigation" aria-label="Main navigation">
        {/* Brand Header */}
        <div className="p-6 border-b border-border">
          <h1 className="text-brand-main font-semibold text-xl">What's New</h1>
          <div className="flex items-center gap-2 mt-2">
            <p className="text-sm text-muted-foreground">{currentUser?.name}</p>
            <Badge variant={isAdmin ? 'default' : 'secondary'} className="text-xs capitalize">
              {currentUser?.role}
            </Badge>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 p-4 overflow-y-auto space-y-1">
          {/* Main Section */}
          <p className="text-xs text-muted-foreground px-4 py-2 uppercase tracking-wider font-medium">Main</p>
          {navLink('/', <Newspaper className="w-5 h-5" />, "What's New")}

          {isAdmin && (
            <>
              <div className="my-4 border-t border-border" />
              <p className="text-xs text-muted-foreground px-4 py-2 uppercase tracking-wider font-medium">Administration</p>
              {navLink('/admin/analytics', <BarChart3 className="w-5 h-5" />, 'Analytics')}
              {navLink('/admin/releases', <Package className="w-5 h-5" />, 'Releases')}
              {navLink('/admin/tags', <Tag className="w-5 h-5" />, 'Tags')}
              {navLink('/admin/clients', <Users className="w-5 h-5" />, 'Clients')}

              <div className="my-4 border-t border-border" />
              <p className="text-xs text-muted-foreground px-4 py-2 uppercase tracking-wider font-medium">Tools</p>
              {navLink('/admin/import-export', <FileSpreadsheet className="w-5 h-5" />, 'Import / Export')}
              {navLink('/admin/integrations', <Database className="w-5 h-5" />, 'Integrations')}
            </>
          )}
        </nav>

        {/* Logout */}
        <div className="p-4 border-t border-border">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 px-4 py-3 rounded-lg text-sm text-muted-foreground hover:text-foreground hover:bg-muted transition-colors w-full"
          >
            <LogOut className="w-5 h-5" />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 overflow-y-auto">
        <main id="main-content" role="main" className="p-8">
          <Outlet />
        </main>
      </div>

      {/* Dev API Panel FAB */}
      {isAdmin && (
        <button
          onClick={() => setShowDevPanel(!showDevPanel)}
          className="fixed bottom-4 right-4 z-50 p-3 bg-brand-main text-white rounded-full shadow-lg hover:bg-brand-main-light transition-colors"
          title="Dev API Panel"
        >
          <Settings className="w-5 h-5" />
        </button>
      )}

      {/* Dev API Panel Overlay */}
      {showDevPanel && isAdmin && (
        <div className="fixed bottom-16 right-4 z-50 bg-brand-main text-white rounded-xl shadow-2xl border border-sidebar-border p-5 w-80">
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <span className="font-medium text-sm">Dev API Panel</span>
              <button onClick={() => setShowDevPanel(false)} className="text-white/60 hover:text-white text-xs">Close</button>
            </div>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-white/60">Mode</span>
                <span className="text-brand-success">{apiConfig.useRealApi ? 'Real API' : 'Mock API'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Latency</span>
                <span className="text-brand-warning">{apiConfig.simulateLatency ? `${apiConfig.minLatency}-${apiConfig.maxLatency}ms` : 'Off'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Error Rate</span>
                <span className="text-brand-warning">{(apiConfig.errorRate * 100).toFixed(0)}%</span>
              </div>
            </div>
            <div className="pt-3 border-t border-white/20">
              <p className="text-xs text-white/60 mb-2">Data Persistence</p>
              <Button
                variant="ghost"
                size="sm"
                onClick={handleReset}
                className="text-brand-error hover:text-white hover:bg-brand-error/20 w-full justify-start"
              >
                <RotateCcw className="w-3 h-3 mr-2" />
                Reset All Data to Defaults
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
