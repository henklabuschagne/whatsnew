import { useState, useMemo } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { ReleaseCard } from './ReleaseCard';
import { Newspaper, Search, Filter, X, Package } from 'lucide-react';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from "./ui/select";
import { Card } from './ui/card';
import { EmptyState } from './EmptyState';
import { useKeyboardShortcuts } from '../hooks/useKeyboardShortcuts';

export function WhatsNew() {
  const { tags, reads } = useAppStore('releases', 'changes', 'tags');

  const [searchTerm, setSearchTerm] = useState('');
  const [selectedChangeType, setSelectedChangeType] = useState<string>('all');
  const [selectedModuleTag, setSelectedModuleTag] = useState<string>('all');
  const [showFilters, setShowFilters] = useState(false);

  const hasActiveFilters = searchTerm !== '' || selectedChangeType !== 'all' || selectedModuleTag !== 'all';

  const moduleTags = useMemo(() => tags.filter(t => t.type === 'module' && t.isActive), [tags]);
  const statistics = reads.getStatistics();

  const filteredReleases = useMemo(() => {
    return reads.getReleasesFiltered({
      searchTerm: searchTerm || undefined,
      changeType: selectedChangeType !== 'all' ? selectedChangeType : undefined,
      moduleTag: selectedModuleTag !== 'all' ? selectedModuleTag : undefined,
    }).filter(r => r.isPublished);
  }, [searchTerm, selectedChangeType, selectedModuleTag, reads]);

  useKeyboardShortcuts([
    {
      key: 'f', ctrl: true,
      callback: () => {
        setShowFilters(prev => !prev);
        setTimeout(() => {
          document.querySelector<HTMLInputElement>('[data-search-input]')?.focus();
        }, 100);
      }
    },
    {
      key: 'Escape',
      callback: () => { if (hasActiveFilters) clearFilters(); },
      preventDefault: false
    }
  ]);

  const clearFilters = () => {
    setSearchTerm('');
    setSelectedChangeType('all');
    setSelectedModuleTag('all');
  };

  return (
    <div className="space-y-6">
      {/* Header with Statistics */}
      <div>
        <div className="flex items-center gap-3 mb-6">
          <div className="p-3 bg-brand-primary-light rounded-lg">
            <Newspaper className="w-6 h-6 text-brand-primary" />
          </div>
          <div>
            <h1 className="text-3xl text-foreground">What's New</h1>
            <p className="text-muted-foreground">Latest product updates and releases</p>
          </div>
        </div>

        <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
          <Card className="p-0">
            <div className="p-6 flex items-center gap-4">
              <div className="p-3 bg-brand-primary-light rounded-lg">
                <Package className="w-6 h-6 text-brand-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Releases</p>
                <p className="text-2xl font-bold text-foreground">{statistics.totalReleases}</p>
              </div>
            </div>
          </Card>
          <Card className="p-0">
            <div className="p-6 flex items-center gap-4">
              <div className="p-3 bg-brand-secondary-light rounded-lg">
                <Newspaper className="w-6 h-6 text-brand-secondary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Changes</p>
                <p className="text-2xl font-bold text-foreground">{statistics.totalChanges}</p>
              </div>
            </div>
          </Card>
          <Card className="p-0 border-l-4 border-l-brand-success">
            <div className="p-6">
              <p className="text-sm text-muted-foreground">New Features</p>
              <p className="text-2xl font-bold text-brand-success">{statistics.newFeatureCount}</p>
            </div>
          </Card>
          <Card className="p-0 border-l-4 border-l-brand-warning">
            <div className="p-6">
              <p className="text-sm text-muted-foreground">Enhancements</p>
              <p className="text-2xl font-bold text-brand-warning">{statistics.enhancementCount}</p>
            </div>
          </Card>
          <Card className="p-0 border-l-4 border-l-brand-error">
            <div className="p-6">
              <p className="text-sm text-muted-foreground">Bug Fixes</p>
              <p className="text-2xl font-bold text-brand-error">{statistics.bugFixCount}</p>
            </div>
          </Card>
        </div>
      </div>

      {/* Search and Filters */}
      <div className="space-y-4">
        <div className="flex flex-col sm:flex-row gap-2">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              data-search-input
              placeholder="Search releases and changes... (Ctrl+F)"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
              aria-label="Search releases and changes"
            />
          </div>
          <Button
            variant="outline"
            onClick={() => setShowFilters(!showFilters)}
            className={hasActiveFilters ? 'border-brand-primary bg-brand-primary-light' : ''}
          >
            <Filter className="w-4 h-4 mr-2" />
            Filters
            {hasActiveFilters && (
              <Badge variant="default" className="ml-2">Active</Badge>
            )}
          </Button>
        </div>

        {showFilters && (
          <Card className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm text-muted-foreground">Change Type</label>
                <Select value={selectedChangeType} onValueChange={setSelectedChangeType}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Types</SelectItem>
                    <SelectItem value="new-feature">New Features</SelectItem>
                    <SelectItem value="enhancement">Enhancements</SelectItem>
                    <SelectItem value="bug-fix">Bug Fixes</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <label className="text-sm text-muted-foreground">Module</label>
                <Select value={selectedModuleTag} onValueChange={setSelectedModuleTag}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Modules</SelectItem>
                    {moduleTags.map(tag => (
                      <SelectItem key={tag.id} value={tag.id}>{tag.label}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="flex justify-end gap-2 mt-4 pt-4 border-t border-border">
              <Button variant="outline" onClick={clearFilters}>
                <X className="w-4 h-4 mr-2" />Clear All
              </Button>
            </div>
          </Card>
        )}

        {hasActiveFilters && (
          <div className="flex items-center gap-2 flex-wrap">
            <span className="text-sm text-muted-foreground">Active filters:</span>
            {searchTerm && (
              <Badge variant="secondary" className="flex items-center gap-1">
                Search: "{searchTerm}"
                <X className="w-3 h-3 cursor-pointer" onClick={() => setSearchTerm('')} />
              </Badge>
            )}
            {selectedChangeType !== 'all' && (
              <Badge variant="secondary" className="flex items-center gap-1">
                Type: {selectedChangeType}
                <X className="w-3 h-3 cursor-pointer" onClick={() => setSelectedChangeType('all')} />
              </Badge>
            )}
            {selectedModuleTag !== 'all' && (
              <Badge variant="secondary" className="flex items-center gap-1">
                Module: {moduleTags.find(t => t.id === selectedModuleTag)?.label}
                <X className="w-3 h-3 cursor-pointer" onClick={() => setSelectedModuleTag('all')} />
              </Badge>
            )}
          </div>
        )}
      </div>

      {/* Releases List */}
      {filteredReleases.length > 0 ? (
        <div className="space-y-6">
          {filteredReleases.map(release => (
            <ReleaseCard key={release.id} release={release} />
          ))}
        </div>
      ) : (
        <EmptyState
          icon={Package}
          title="No releases found"
          description={hasActiveFilters ? 'Try adjusting your filters to see more results' : 'No releases have been published yet'}
          actionLabel={hasActiveFilters ? 'Clear Filters' : undefined}
          onAction={hasActiveFilters ? clearFilters : undefined}
        />
      )}
    </div>
  );
}