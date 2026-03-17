import { useState, useEffect } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import type { Release, Change, ChangeType } from '../types';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Textarea } from './ui/textarea';
import { Label } from './ui/label';
import { Badge } from './ui/badge';
import { Plus, X } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { Checkbox } from './ui/checkbox';
import { toast } from "sonner@2.0.3";

interface ReleaseFormProps {
  releaseId: string | null;
  onSave: () => void;
  onCancel: () => void;
}

interface ChangeFormData {
  title: string;
  description: string;
  changeType: ChangeType;
  moduleTags: string[];
  clientId: string;
  ticketNumber: string;
  devopsNumber: string;
}

export function ReleaseForm({ releaseId, onSave, onCancel }: ReleaseFormProps) {
  const { tags, clients, reads, actions } = useAppStore('releases', 'changes', 'tags', 'clients');

  const moduleTags = tags.filter(t => t.type === 'module');
  const activeClients = clients.filter(c => c.isActive);

  const [version, setVersion] = useState('');
  const [releaseDate, setReleaseDate] = useState(new Date().toISOString().split('T')[0]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // Inline change additions
  const [showChangeForm, setShowChangeForm] = useState(false);
  const [changeForm, setChangeForm] = useState<ChangeFormData>({
    title: '', description: '', changeType: 'new-feature',
    moduleTags: [], clientId: '', ticketNumber: '', devopsNumber: '',
  });

  useEffect(() => {
    if (releaseId) {
      const r = reads.getReleaseById(releaseId);
      if (r) {
        setVersion(r.version);
        setReleaseDate(r.releaseDate);
        setTitle(r.title);
        setDescription(r.description);
      }
    } else {
      setVersion('');
      setReleaseDate(new Date().toISOString().split('T')[0]);
      setTitle('');
      setDescription('');
    }
  }, [releaseId]);

  const existingChanges = releaseId ? reads.getChangesByReleaseId(releaseId) : [];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!version.trim()) { toast.error('Version is required'); return; }

    setSubmitting(true);
    if (releaseId) {
      const result = await actions.updateRelease(releaseId, { version, releaseDate, title, description });
      if (result.success) { toast.success('Release updated'); onSave(); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createRelease({ version, releaseDate, title, description, isPublished: true });
      if (result.success) { toast.success('Release created'); onSave(); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleAddChange = async () => {
    if (!releaseId) { toast.error('Save the release first before adding changes'); return; }
    if (!changeForm.title.trim()) { toast.error('Change title is required'); return; }
    if (!changeForm.description.trim()) { toast.error('Change description is required'); return; }

    const result = await actions.createChange({
      releaseId,
      title: changeForm.title,
      description: changeForm.description,
      changeType: changeForm.changeType,
      moduleTags: changeForm.moduleTags,
      clientId: changeForm.clientId || undefined,
      ticketNumber: changeForm.ticketNumber || undefined,
      devopsNumber: changeForm.devopsNumber || undefined,
    });
    if (result.success) {
      toast.success('Change added');
      setChangeForm({ title: '', description: '', changeType: 'new-feature', moduleTags: [], clientId: '', ticketNumber: '', devopsNumber: '' });
      setShowChangeForm(false);
    } else {
      toast.error(result.error.message);
    }
  };

  const handleDeleteChange = async (changeId: string) => {
    const result = await actions.deleteChange(changeId);
    if (result.success) toast.success('Change removed');
    else toast.error(result.error.message);
  };

  const toggleModuleTag = (tagValue: string) => {
    setChangeForm(prev => ({
      ...prev,
      moduleTags: prev.moduleTags.includes(tagValue)
        ? prev.moduleTags.filter(t => t !== tagValue)
        : [...prev.moduleTags, tagValue],
    }));
  };

  const getChangeTypeLabel = (type: ChangeType): string => {
    switch (type) {
      case 'bug-fix': return 'Bug Fix';
      case 'new-feature': return 'New Feature';
      case 'enhancement': return 'Enhancement';
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>{releaseId ? 'Edit Release' : 'New Release'}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="version">Version</Label>
              <Input
                id="version"
                placeholder="e.g., 2.1.0"
                value={version}
                onChange={(e) => setVersion(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="releaseDate">Release Date</Label>
              <Input
                id="releaseDate"
                type="date"
                value={releaseDate}
                onChange={(e) => setReleaseDate(e.target.value)}
                required
              />
            </div>
          </div>
          <div className="space-y-2">
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              placeholder="Release title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Release description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>
        </CardContent>
      </Card>

      {/* Changes Section (only shown when editing an existing release) */}
      {releaseId && (
        <div className="space-y-4">
          <div className="flex justify-between items-center">
            <h3 className="text-foreground">Changes ({existingChanges.length})</h3>
            <Button type="button" onClick={() => setShowChangeForm(true)} size="sm">
              <Plus className="w-4 h-4 mr-2" />
              Add Change
            </Button>
          </div>

          {existingChanges.map((change) => (
            <Card key={change.id}>
              <CardContent className="pt-4">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <p className="text-foreground text-sm">{change.title}</p>
                    <p className="text-muted-foreground text-xs mt-1">{change.description}</p>
                    <div className="flex gap-1 mt-2">
                      <Badge variant="outline" className="text-xs">{getChangeTypeLabel(change.changeType)}</Badge>
                      {change.moduleTags.map((tag, i) => {
                        const td = moduleTags.find(t => t.value === tag);
                        return <Badge key={`${change.id}-${tag}-${i}`} variant="secondary" className="text-xs">{td?.label || tag}</Badge>;
                      })}
                    </div>
                  </div>
                  <Button type="button" variant="ghost" size="sm" onClick={() => handleDeleteChange(change.id)}>
                    <X className="w-4 h-4 text-destructive" />
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}

          {existingChanges.length === 0 && !showChangeForm && (
            <div className="text-center py-8 bg-muted rounded-lg border-2 border-dashed border-border">
              <p className="text-muted-foreground mb-4">No changes added yet</p>
              <Button type="button" onClick={() => setShowChangeForm(true)} variant="outline">
                <Plus className="w-4 h-4 mr-2" />
                Add Your First Change
              </Button>
            </div>
          )}

          {/* Inline Add Change Form */}
          {showChangeForm && (
            <Card className="border-brand-primary bg-brand-primary-light/30">
              <CardContent className="pt-6 space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-foreground">New Change</span>
                  <Button type="button" variant="ghost" size="sm" onClick={() => setShowChangeForm(false)}>
                    <X className="w-4 h-4" />
                  </Button>
                </div>

                <div className="space-y-2">
                  <Label>Title</Label>
                  <Input
                    placeholder="Change title"
                    value={changeForm.title}
                    onChange={(e) => setChangeForm(f => ({ ...f, title: e.target.value }))}
                  />
                </div>

                <div className="space-y-2">
                  <Label>Description</Label>
                  <Textarea
                    placeholder="Describe the change..."
                    value={changeForm.description}
                    onChange={(e) => setChangeForm(f => ({ ...f, description: e.target.value }))}
                  />
                </div>

                <div className="space-y-2">
                  <Label>Change Type</Label>
                  <Select
                    value={changeForm.changeType}
                    onValueChange={(value) => setChangeForm(f => ({ ...f, changeType: value as ChangeType }))}
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="new-feature">New Feature</SelectItem>
                      <SelectItem value="enhancement">Enhancement</SelectItem>
                      <SelectItem value="bug-fix">Bug Fix</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
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

                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="space-y-2">
                    <Label>Client (Optional)</Label>
                    <Select
                      value={changeForm.clientId || 'none'}
                      onValueChange={(value) => setChangeForm(f => ({ ...f, clientId: value === 'none' ? '' : value }))}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Select client..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="none">No client</SelectItem>
                        {activeClients.map(client => (
                          <SelectItem key={client.id} value={client.id}>
                            {client.name} ({client.code})
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  <div className="space-y-2">
                    <Label>Ticket Number (Optional)</Label>
                    <Input
                      placeholder="e.g., TICKET-1234"
                      value={changeForm.ticketNumber}
                      onChange={(e) => setChangeForm(f => ({ ...f, ticketNumber: e.target.value }))}
                    />
                  </div>

                  <div className="space-y-2">
                    <Label>DevOps Number (Optional)</Label>
                    <Input
                      placeholder="e.g., DEV-5678"
                      value={changeForm.devopsNumber}
                      onChange={(e) => setChangeForm(f => ({ ...f, devopsNumber: e.target.value }))}
                    />
                  </div>
                </div>

                <div className="flex justify-end gap-2 pt-2">
                  <Button type="button" variant="outline" onClick={() => setShowChangeForm(false)}>Cancel</Button>
                  <Button type="button" onClick={handleAddChange}>Add Change</Button>
                </div>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" disabled={submitting}>
          {releaseId ? 'Update Release' : 'Create Release'}
        </Button>
      </div>
    </form>
  );
}