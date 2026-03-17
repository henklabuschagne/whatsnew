import { useState } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Plus, Pencil, Trash2, Loader2, Tag as TagIcon } from 'lucide-react';
import { Input } from './ui/input';
import { Button } from './ui/button';
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
import { Badge } from './ui/badge';
import { Switch } from './ui/switch';
import { Card } from './ui/card';

export function TagManagement() {
  const { tags, actions } = useAppStore('tags');

  const moduleTags = tags.filter(t => t.type === 'module');
  const changeTypeTags = tags.filter(t => t.type === 'change-type');

  const [showDialog, setShowDialog] = useState(false);
  const [editingTagId, setEditingTagId] = useState<string | null>(null);
  const [form, setForm] = useState({ label: '', value: '', type: 'module' as string });
  const [deleteTagId, setDeleteTagId] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleNew = (type: string = 'module') => {
    setEditingTagId(null);
    setForm({ label: '', value: '', type });
    setShowDialog(true);
  };

  const handleEdit = (id: string) => {
    const tag = tags.find(t => t.id === id);
    if (!tag) return;
    setEditingTagId(id);
    setForm({ label: tag.label, value: tag.value, type: tag.type });
    setShowDialog(true);
  };

  const handleSave = async () => {
    if (!form.label.trim()) { toast.error('Label is required'); return; }
    setSubmitting(true);
    if (editingTagId) {
      const result = await actions.updateTag(editingTagId, { label: form.label });
      if (result.success) { toast.success('Tag updated'); setShowDialog(false); }
      else toast.error(result.error.message);
    } else {
      const value = form.value.trim() || form.label.toLowerCase().replace(/\s+/g, '-');
      const result = await actions.createTag({ label: form.label, value, type: form.type });
      if (result.success) { toast.success('Tag created'); setShowDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDelete = async () => {
    if (!deleteTagId) return;
    setSubmitting(true);
    const result = await actions.deleteTag(deleteTagId);
    if (result.success) toast.success('Tag deleted');
    else toast.error(result.error.message);
    setDeleteTagId(null);
    setSubmitting(false);
  };

  const handleToggleActive = async (id: string, isActive: boolean) => {
    const result = await actions.updateTag(id, { isActive });
    if (result.success) toast.success(`Tag ${isActive ? 'enabled' : 'disabled'}`);
    else toast.error(result.error.message);
  };

  const renderTagSection = (title: string, tagList: typeof tags, type: string) => (
    <Card>
      <div className="p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-foreground">{title}</h2>
          <Button variant="outline" size="sm" onClick={() => handleNew(type)}>
            <Plus className="w-4 h-4 mr-1" />Add
          </Button>
        </div>
        {tagList.length === 0 ? (
          <p className="text-muted-foreground text-sm text-center py-4">No tags in this category</p>
        ) : (
          <div className="space-y-2">
            {tagList.map(tag => (
              <div key={tag.id} className="flex items-center justify-between p-3 border rounded-lg">
                <div className="flex items-center gap-3">
                  <Switch checked={tag.isActive} onCheckedChange={(checked) => handleToggleActive(tag.id, checked)} />
                  <div>
                    <span className={`text-sm ${tag.isActive ? 'text-foreground' : 'text-muted-foreground'}`}>{tag.label}</span>
                    <span className="text-xs text-muted-foreground ml-2">({tag.value})</span>
                  </div>
                  {!tag.isActive && <Badge variant="secondary" className="text-xs">Disabled</Badge>}
                </div>
                <div className="flex gap-1">
                  <Button variant="ghost" size="sm" onClick={() => handleEdit(tag.id)}><Pencil className="w-3 h-3" /></Button>
                  <Button variant="ghost" size="sm" onClick={() => setDeleteTagId(tag.id)} className="text-destructive"><Trash2 className="w-3 h-3" /></Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Card>
  );

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl text-foreground mb-2">Tag Management</h1>
        <p className="text-muted-foreground">Manage module tags and change type tags</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {renderTagSection('Module Tags', moduleTags, 'module')}
        {renderTagSection('Change Type Tags', changeTypeTags, 'change-type')}
      </div>

      {/* Dialog */}
      <Dialog open={showDialog} onOpenChange={setShowDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingTagId ? 'Edit Tag' : 'New Tag'}</DialogTitle>
            <DialogDescription>Enter the tag details below.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Label</Label><Input value={form.label} onChange={e => setForm(f => ({ ...f, label: e.target.value }))} placeholder="e.g. Import" /></div>
            {!editingTagId && (
              <>
                <div><Label>Value (slug)</Label><Input value={form.value} onChange={e => setForm(f => ({ ...f, value: e.target.value }))} placeholder="Auto-generated from label" /></div>
                <div>
                  <Label>Type</Label>
                  <Select value={form.type} onValueChange={v => setForm(f => ({ ...f, type: v }))}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="module">Module</SelectItem>
                      <SelectItem value="change-type">Change Type</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowDialog(false)}>Cancel</Button>
            <Button onClick={handleSave} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingTagId ? 'Update' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirm */}
      <AlertDialog open={!!deleteTagId} onOpenChange={() => setDeleteTagId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Tag</AlertDialogTitle>
            <AlertDialogDescription>This will permanently delete this tag. Existing changes using this tag will not be affected.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={handleDelete} disabled={submitting} className="bg-red-600 hover:bg-red-700">
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}