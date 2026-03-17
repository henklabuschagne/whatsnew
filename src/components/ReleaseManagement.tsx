import { useState } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Plus, Pencil, Trash2, Calendar, Loader2, ChevronDown, ChevronRight, Package, User, Ticket, GitBranch, Sparkles, Bug, Zap } from 'lucide-react';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Textarea } from './ui/textarea';
import { EmptyState } from './EmptyState';
import {
  AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent,
  AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle,
} from "./ui/alert-dialog";
import {
  Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogFooter,
} from "./ui/dialog";
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from "./ui/select";
import { Label } from './ui/label';
import { toast } from "sonner@2.0.3";
import { Checkbox } from './ui/checkbox';

export function ReleaseManagement() {
  const { releases, changes, tags, clients, reads, actions } = useAppStore('releases', 'changes', 'tags', 'clients');

  const moduleTags = tags.filter(t => t.type === 'module');
  const releasesWithChanges = reads.getReleasesWithChanges();

  const [expandedReleases, setExpandedReleases] = useState<Set<string>>(new Set());
  const [showReleaseDialog, setShowReleaseDialog] = useState(false);
  const [editingReleaseId, setEditingReleaseId] = useState<string | null>(null);
  const [releaseForm, setReleaseForm] = useState({ version: '', releaseDate: new Date().toISOString().split('T')[0], title: '', description: '' });

  const [showChangeDialog, setShowChangeDialog] = useState(false);
  const [editingChangeId, setEditingChangeId] = useState<string | null>(null);
  const [selectedReleaseId, setSelectedReleaseId] = useState('');
  const [changeForm, setChangeForm] = useState({ title: '', description: '', changeType: 'new-feature', moduleTags: [] as string[], clientId: '', ticketNumber: '', devopsNumber: '' });

  const [deleteReleaseId, setDeleteReleaseId] = useState<string | null>(null);
  const [deleteChangeId, setDeleteChangeId] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const toggleRelease = (id: string) => {
    const next = new Set(expandedReleases);
    next.has(id) ? next.delete(id) : next.add(id);
    setExpandedReleases(next);
  };

  // Release CRUD
  const handleNewRelease = () => {
    setEditingReleaseId(null);
    setReleaseForm({ version: '', releaseDate: new Date().toISOString().split('T')[0], title: '', description: '' });
    setShowReleaseDialog(true);
  };

  const handleEditRelease = (id: string) => {
    const r = reads.getReleaseById(id);
    if (!r) return;
    setEditingReleaseId(id);
    setReleaseForm({ version: r.version, releaseDate: r.releaseDate, title: r.title, description: r.description });
    setShowReleaseDialog(true);
  };

  const handleSaveRelease = async () => {
    if (!releaseForm.version.trim()) { toast.error('Version is required'); return; }
    setSubmitting(true);
    if (editingReleaseId) {
      const result = await actions.updateRelease(editingReleaseId, releaseForm);
      if (result.success) { toast.success('Release updated'); setShowReleaseDialog(false); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createRelease({ ...releaseForm, isPublished: true });
      if (result.success) { toast.success('Release created'); setShowReleaseDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDeleteRelease = async () => {
    if (!deleteReleaseId) return;
    setSubmitting(true);
    const result = await actions.deleteRelease(deleteReleaseId);
    if (result.success) toast.success('Release deleted');
    else toast.error(result.error.message);
    setDeleteReleaseId(null);
    setSubmitting(false);
  };

  // Change CRUD
  const handleNewChange = (releaseId: string) => {
    setEditingChangeId(null);
    setSelectedReleaseId(releaseId);
    setChangeForm({ title: '', description: '', changeType: 'new-feature', moduleTags: [], clientId: '', ticketNumber: '', devopsNumber: '' });
    setShowChangeDialog(true);
  };

  const handleEditChange = (changeId: string) => {
    const c = reads.getChangeById(changeId);
    if (!c) return;
    setEditingChangeId(changeId);
    setSelectedReleaseId(c.releaseId);
    setChangeForm({
      title: c.title, description: c.description, changeType: c.changeType,
      moduleTags: [...c.moduleTags], clientId: c.clientId || '', ticketNumber: c.ticketNumber || '', devopsNumber: c.devopsNumber || '',
    });
    setShowChangeDialog(true);
  };

  const handleSaveChange = async () => {
    if (!changeForm.title.trim()) { toast.error('Title is required'); return; }
    if (!changeForm.description.trim()) { toast.error('Description is required'); return; }
    setSubmitting(true);
    if (editingChangeId) {
      const result = await actions.updateChange(editingChangeId, {
        ...changeForm,
        clientId: changeForm.clientId || undefined,
        ticketNumber: changeForm.ticketNumber || undefined,
        devopsNumber: changeForm.devopsNumber || undefined,
      });
      if (result.success) { toast.success('Change updated'); setShowChangeDialog(false); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createChange({
        releaseId: selectedReleaseId,
        ...changeForm,
        clientId: changeForm.clientId || undefined,
        ticketNumber: changeForm.ticketNumber || undefined,
        devopsNumber: changeForm.devopsNumber || undefined,
      });
      if (result.success) { toast.success('Change created'); setShowChangeDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDeleteChange = async () => {
    if (!deleteChangeId) return;
    setSubmitting(true);
    const result = await actions.deleteChange(deleteChangeId);
    if (result.success) toast.success('Change deleted');
    else toast.error(result.error.message);
    setDeleteChangeId(null);
    setSubmitting(false);
  };

  const toggleModuleTag = (tagValue: string) => {
    setChangeForm(prev => ({
      ...prev,
      moduleTags: prev.moduleTags.includes(tagValue)
        ? prev.moduleTags.filter(t => t !== tagValue)
        : [...prev.moduleTags, tagValue],
    }));
  };

  const getChangeTypeIcon = (type: string) => {
    switch (type) {
      case 'bug-fix': return <Bug className="w-4 h-4 text-brand-error" />;
      case 'new-feature': return <Sparkles className="w-4 h-4 text-brand-success" />;
      case 'enhancement': return <Zap className="w-4 h-4 text-brand-primary" />;
      default: return null;
    }
  };

  const formatDate = (d: string) => new Date(d).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl text-foreground mb-2">Release Management</h1>
          <p className="text-muted-foreground">Manage releases and their changes</p>
        </div>
        <Button onClick={handleNewRelease}>
          <Plus className="w-4 h-4 mr-2" />New Release
        </Button>
      </div>

      {releasesWithChanges.length === 0 ? (
        <EmptyState icon={Package} title="No releases yet" description="Create your first release to get started" actionLabel="Create Release" onAction={handleNewRelease} />
      ) : (
        <div className="space-y-4">
          {releasesWithChanges.map(release => (
            <div key={release.id} className="bg-card rounded-xl border border-border">
              <div className="p-4 flex items-center justify-between">
                <button onClick={() => toggleRelease(release.id)} className="flex items-center gap-3 flex-1 text-left">
                  {expandedReleases.has(release.id) ? <ChevronDown className="w-5 h-5 text-muted-foreground" /> : <ChevronRight className="w-5 h-5 text-muted-foreground" />}
                  <div>
                    <div className="flex items-center gap-2">
                      <span className="text-foreground">v{release.version}</span>
                      <span className="text-xs text-muted-foreground">{release.title}</span>
                    </div>
                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                      <Calendar className="w-3 h-3" />{formatDate(release.releaseDate)}
                      <span>· {release.changes.length} changes</span>
                    </div>
                  </div>
                </button>
                <div className="flex gap-2">
                  <Button variant="ghost" size="sm" onClick={() => handleEditRelease(release.id)}><Pencil className="w-4 h-4" /></Button>
                  <Button variant="ghost" size="sm" onClick={() => setDeleteReleaseId(release.id)} className="text-destructive hover:text-destructive"><Trash2 className="w-4 h-4" /></Button>
                </div>
              </div>

              {expandedReleases.has(release.id) && (
                <div className="border-t border-border p-4">
                  <div className="flex justify-between items-center mb-3">
                    <h3 className="text-foreground text-sm">Changes ({release.changes.length})</h3>
                    <Button variant="outline" size="sm" onClick={() => handleNewChange(release.id)}>
                      <Plus className="w-3 h-3 mr-1" />Add Change
                    </Button>
                  </div>
                  {release.changes.length === 0 ? (
                    <p className="text-muted-foreground text-sm py-4 text-center">No changes in this release yet</p>
                  ) : (
                    <div className="space-y-2">
                      {release.changes.map(change => (
                        <div key={change.id} className="flex items-start gap-3 p-3 bg-muted/50 rounded-lg">
                          {getChangeTypeIcon(change.changeType)}
                          <div className="flex-1 min-w-0">
                            <p className="text-foreground text-sm">{change.title}</p>
                            <p className="text-muted-foreground text-xs mt-1">{change.description}</p>
                            <div className="flex flex-wrap gap-1 mt-2">
                              {change.moduleTags.map((tag, i) => {
                                const td = moduleTags.find(t => t.value === tag);
                                return <span key={`${change.id}-${tag}-${i}`} className="px-1.5 py-0.5 text-xs bg-secondary text-secondary-foreground rounded">{td?.label || tag}</span>;
                              })}
                            </div>
                            {(change.clientId || change.ticketNumber || change.devopsNumber) && (
                              <div className="flex gap-3 mt-1 text-xs text-muted-foreground">
                                {change.clientId && <span className="flex items-center gap-1"><User className="w-3 h-3" />{clients.find(c => c.id === change.clientId)?.name || change.clientId}</span>}
                                {change.ticketNumber && <span className="flex items-center gap-1"><Ticket className="w-3 h-3" />{change.ticketNumber}</span>}
                                {change.devopsNumber && <span className="flex items-center gap-1"><GitBranch className="w-3 h-3" />{change.devopsNumber}</span>}
                              </div>
                            )}
                          </div>
                          <div className="flex gap-1">
                            <Button variant="ghost" size="sm" onClick={() => handleEditChange(change.id)}><Pencil className="w-3 h-3" /></Button>
                            <Button variant="ghost" size="sm" onClick={() => setDeleteChangeId(change.id)} className="text-destructive"><Trash2 className="w-3 h-3" /></Button>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {/* Release Dialog */}
      <Dialog open={showReleaseDialog} onOpenChange={setShowReleaseDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingReleaseId ? 'Edit Release' : 'New Release'}</DialogTitle>
            <DialogDescription>Enter the release details below.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Version</Label><Input value={releaseForm.version} onChange={e => setReleaseForm(f => ({ ...f, version: e.target.value }))} placeholder="e.g. 2.6.0" /></div>
            <div><Label>Release Date</Label><Input type="date" value={releaseForm.releaseDate} onChange={e => setReleaseForm(f => ({ ...f, releaseDate: e.target.value }))} /></div>
            <div><Label>Title</Label><Input value={releaseForm.title} onChange={e => setReleaseForm(f => ({ ...f, title: e.target.value }))} placeholder="Release title" /></div>
            <div><Label>Description</Label><Textarea value={releaseForm.description} onChange={e => setReleaseForm(f => ({ ...f, description: e.target.value }))} placeholder="Release description" /></div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowReleaseDialog(false)}>Cancel</Button>
            <Button onClick={handleSaveRelease} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingReleaseId ? 'Update' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Change Dialog */}
      <Dialog open={showChangeDialog} onOpenChange={setShowChangeDialog}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>{editingChangeId ? 'Edit Change' : 'Add Change'}</DialogTitle>
            <DialogDescription>Enter the change details below.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Title</Label><Input value={changeForm.title} onChange={e => setChangeForm(f => ({ ...f, title: e.target.value }))} placeholder="Change title" /></div>
            <div><Label>Description</Label><Textarea value={changeForm.description} onChange={e => setChangeForm(f => ({ ...f, description: e.target.value }))} placeholder="Detailed description" /></div>
            <div>
              <Label>Change Type</Label>
              <Select value={changeForm.changeType} onValueChange={v => setChangeForm(f => ({ ...f, changeType: v }))}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="new-feature">New Feature</SelectItem>
                  <SelectItem value="enhancement">Enhancement</SelectItem>
                  <SelectItem value="bug-fix">Bug Fix</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Module Tags</Label>
              <div className="flex flex-wrap gap-2 mt-1">
                {moduleTags.map(tag => (
                  <label key={tag.id} className="flex items-center gap-1.5 text-sm">
                    <Checkbox checked={changeForm.moduleTags.includes(tag.value)} onCheckedChange={() => toggleModuleTag(tag.value)} />
                    {tag.label}
                  </label>
                ))}
              </div>
            </div>
            <div>
              <Label>Client</Label>
              <Select value={changeForm.clientId || 'none'} onValueChange={v => setChangeForm(f => ({ ...f, clientId: v === 'none' ? '' : v }))}>
                <SelectTrigger><SelectValue placeholder="Select client" /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">No client</SelectItem>
                  {clients.filter(c => c.isActive).map(c => (
                    <SelectItem key={c.id} value={c.id}>{c.name} ({c.code})</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div><Label>Ticket #</Label><Input value={changeForm.ticketNumber} onChange={e => setChangeForm(f => ({ ...f, ticketNumber: e.target.value }))} placeholder="TICKET-12345" /></div>
              <div><Label>DevOps #</Label><Input value={changeForm.devopsNumber} onChange={e => setChangeForm(f => ({ ...f, devopsNumber: e.target.value }))} placeholder="DEVOPS-13456" /></div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowChangeDialog(false)}>Cancel</Button>
            <Button onClick={handleSaveChange} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingChangeId ? 'Update' : 'Add'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Release Confirm */}
      <AlertDialog open={!!deleteReleaseId} onOpenChange={() => setDeleteReleaseId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Release</AlertDialogTitle>
            <AlertDialogDescription>This will permanently delete this release and all its changes. This action cannot be undone.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={handleDeleteRelease} disabled={submitting} className="bg-red-600 hover:bg-red-700">
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Delete Change Confirm */}
      <AlertDialog open={!!deleteChangeId} onOpenChange={() => setDeleteChangeId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Change</AlertDialogTitle>
            <AlertDialogDescription>This will permanently delete this change entry.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={handleDeleteChange} disabled={submitting} className="bg-red-600 hover:bg-red-700">
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}