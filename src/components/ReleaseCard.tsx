import { useState, useMemo } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Calendar, Filter, Sparkles, Bug, Zap, ChevronDown, ChevronRight, User, Ticket, GitBranch } from 'lucide-react';
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from "./ui/select";
import type { ReleaseWithChanges, ChangeType } from '../types';

interface ReleaseCardProps {
  release: ReleaseWithChanges;
}

export function ReleaseCard({ release }: ReleaseCardProps) {
  const { tags, clients } = useAppStore('tags', 'clients');
  const [isExpanded, setIsExpanded] = useState(false);
  const [selectedChangeType, setSelectedChangeType] = useState<string>('all');
  const [selectedModule, setSelectedModule] = useState<string>('all');

  const moduleTags = useMemo(() => tags.filter(t => t.type === 'module'), [tags]);

  const getClientName = (clientId?: string) => {
    if (!clientId) return null;
    const client = clients.find(c => c.id === clientId);
    return client ? `${client.name} (${client.code})` : null;
  };

  const getChangeTypeColor = (type: ChangeType): string => {
    switch (type) {
      case 'bug-fix': return 'border-l-brand-error bg-brand-error-light';
      case 'new-feature': return 'border-l-brand-success bg-brand-success-light';
      case 'enhancement': return 'border-l-brand-primary bg-brand-primary-light';
    }
  };

  const getChangeTypeLabel = (type: ChangeType): string => {
    switch (type) {
      case 'bug-fix': return 'Bug Fix';
      case 'new-feature': return 'New Feature';
      case 'enhancement': return 'Enhancement';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  };

  const changeTypesInRelease = Array.from(new Set(release.changes.map(c => c.changeType)));
  const modulesInRelease = Array.from(new Set(release.changes.flatMap(c => c.moduleTags)));

  const filteredChanges = release.changes.filter(change => {
    const matchesType = selectedChangeType === 'all' || change.changeType === selectedChangeType;
    const matchesModule = selectedModule === 'all' || change.moduleTags.includes(selectedModule);
    return matchesType && matchesModule;
  });

  const allChangesByType = {
    'new-feature': release.changes.filter(c => c.changeType === 'new-feature'),
    'enhancement': release.changes.filter(c => c.changeType === 'enhancement'),
    'bug-fix': release.changes.filter(c => c.changeType === 'bug-fix'),
  };

  const changesByType = {
    'new-feature': filteredChanges.filter(c => c.changeType === 'new-feature'),
    'enhancement': filteredChanges.filter(c => c.changeType === 'enhancement'),
    'bug-fix': filteredChanges.filter(c => c.changeType === 'bug-fix'),
  };

  const renderChangeGroup = (type: ChangeType, icon: React.ReactNode, label: string, changes: typeof filteredChanges) => {
    if (changes.length === 0) return null;
    return (
      <div key={`${type}-section`}>
        <div className="flex items-center gap-2 mb-3">
          {icon}
          <h3 className="text-foreground">{label}</h3>
          <span className="text-sm text-muted-foreground">({changes.length})</span>
        </div>
        <div className="space-y-3">
          {changes.map(change => (
            <div key={change.id} className={`border-l-4 pl-4 py-3 rounded-r-lg ${getChangeTypeColor(change.changeType)}`}>
              <p className="text-foreground mb-1">{change.title}</p>
              <p className="text-muted-foreground text-sm mb-2">{change.description}</p>
              <div className="flex flex-wrap gap-1.5 mb-2">
                {change.moduleTags.map((tag, i) => {
                  const tagData = moduleTags.find(t => t.value === tag);
                  return (
                    <span key={`${change.id}-${tag}-${i}`} className="px-2 py-0.5 text-xs bg-card border border-border text-foreground rounded capitalize">
                      {tagData?.label || tag}
                    </span>
                  );
                })}
              </div>
              {(change.clientId || change.ticketNumber || change.devopsNumber) && (
                <div className="flex flex-wrap gap-3 text-xs text-muted-foreground mt-2">
                  {change.clientId && getClientName(change.clientId) && (
                    <div className="flex items-center gap-1"><User className="w-3 h-3" /><span>{getClientName(change.clientId)}</span></div>
                  )}
                  {change.ticketNumber && (
                    <div className="flex items-center gap-1"><Ticket className="w-3 h-3" /><span>{change.ticketNumber}</span></div>
                  )}
                  {change.devopsNumber && (
                    <div className="flex items-center gap-1"><GitBranch className="w-3 h-3" /><span>{change.devopsNumber}</span></div>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    );
  };

  return (
    <div className="bg-card rounded-xl border border-border hover:shadow-lg transition-shadow">
      <div className="w-full p-6">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <div className="flex items-center gap-3 mb-2">
              <button onClick={() => setIsExpanded(!isExpanded)} className="flex items-center gap-2 hover:opacity-70 transition-opacity">
                {isExpanded ? <ChevronDown className="w-5 h-5 text-muted-foreground" /> : <ChevronRight className="w-5 h-5 text-muted-foreground" />}
                <h2 className="text-foreground">Version {release.version}</h2>
              </button>
              <div className="flex gap-1">
                {allChangesByType['new-feature'].length > 0 && (
                  <button onClick={(e) => { e.stopPropagation(); setIsExpanded(true); setSelectedChangeType('new-feature'); }} className="px-2 py-1 bg-brand-success-light text-brand-success text-xs rounded-md flex items-center gap-1 hover:bg-brand-success-mid transition-colors cursor-pointer">
                    <Sparkles className="w-3 h-3" />{allChangesByType['new-feature'].length} new
                  </button>
                )}
                {allChangesByType['enhancement'].length > 0 && (
                  <button onClick={(e) => { e.stopPropagation(); setIsExpanded(true); setSelectedChangeType('enhancement'); }} className="px-2 py-1 bg-brand-primary-light text-brand-primary text-xs rounded-md flex items-center gap-1 hover:bg-accent transition-colors cursor-pointer">
                    <Zap className="w-3 h-3" />{allChangesByType['enhancement'].length} improved
                  </button>
                )}
                {allChangesByType['bug-fix'].length > 0 && (
                  <button onClick={(e) => { e.stopPropagation(); setIsExpanded(true); setSelectedChangeType('bug-fix'); }} className="px-2 py-1 bg-brand-error-light text-brand-error text-xs rounded-md flex items-center gap-1 hover:bg-brand-error-mid transition-colors cursor-pointer">
                    <Bug className="w-3 h-3" />{allChangesByType['bug-fix'].length} fixed
                  </button>
                )}
              </div>
            </div>
            <div className="flex items-center gap-2 text-muted-foreground ml-7">
              <Calendar className="w-4 h-4" />
              <span>{formatDate(release.releaseDate)}</span>
            </div>
          </div>
        </div>
      </div>

      {isExpanded && (
        <>
          <div className="px-6 pb-4 border-b border-border bg-muted">
            <div className="flex items-center gap-3">
              <Filter className="w-4 h-4 text-muted-foreground" />
              <div className="flex gap-2 flex-1">
                <Select value={selectedChangeType} onValueChange={setSelectedChangeType}>
                  <SelectTrigger className="w-[180px] bg-card"><SelectValue placeholder="All types" /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All types</SelectItem>
                    {changeTypesInRelease.map(type => (
                      <SelectItem key={type} value={type}>{getChangeTypeLabel(type as ChangeType)}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <Select value={selectedModule} onValueChange={setSelectedModule}>
                  <SelectTrigger className="w-[180px] bg-card"><SelectValue placeholder="All modules" /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All modules</SelectItem>
                    {modulesInRelease.map(mod => {
                      const tag = moduleTags.find(t => t.value === mod);
                      return <SelectItem key={mod} value={mod}>{tag?.label || mod}</SelectItem>;
                    })}
                  </SelectContent>
                </Select>
              </div>
              {(selectedChangeType !== 'all' || selectedModule !== 'all') && (
                <button onClick={() => { setSelectedChangeType('all'); setSelectedModule('all'); }} className="text-sm text-brand-primary hover:underline">Clear filters</button>
              )}
            </div>
          </div>

          <div className="p-6 space-y-6">
            {renderChangeGroup('new-feature', <Sparkles className="w-5 h-5 text-brand-success" />, 'New Features', changesByType['new-feature'])}
            {renderChangeGroup('enhancement', <Zap className="w-5 h-5 text-brand-primary" />, 'Enhancements', changesByType['enhancement'])}
            {renderChangeGroup('bug-fix', <Bug className="w-5 h-5 text-brand-error" />, 'Bug Fixes', changesByType['bug-fix'])}
            {filteredChanges.length === 0 && (
              <div className="text-center py-8 text-muted-foreground">No changes match the selected filters</div>
            )}
          </div>
        </>
      )}
    </div>
  );
}