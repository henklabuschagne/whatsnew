import { useState } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Plus, Pencil, Trash2, Loader2, Users } from 'lucide-react';
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
import { Label } from './ui/label';
import { toast } from "sonner@2.0.3";
import { Badge } from './ui/badge';
import { Switch } from './ui/switch';
import { Card } from './ui/card';

export function ClientManagement() {
  const { clients, actions } = useAppStore('clients');

  const [showDialog, setShowDialog] = useState(false);
  const [editingClientId, setEditingClientId] = useState<string | null>(null);
  const [form, setForm] = useState({ name: '', code: '', description: '' });
  const [deleteClientId, setDeleteClientId] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleNew = () => {
    setEditingClientId(null);
    setForm({ name: '', code: '', description: '' });
    setShowDialog(true);
  };

  const handleEdit = (id: string) => {
    const c = clients.find(cl => cl.id === id);
    if (!c) return;
    setEditingClientId(id);
    setForm({ name: c.name, code: c.code, description: c.description || '' });
    setShowDialog(true);
  };

  const handleSave = async () => {
    if (!form.name.trim()) { toast.error('Name is required'); return; }
    if (!form.code.trim()) { toast.error('Code is required'); return; }
    setSubmitting(true);
    if (editingClientId) {
      const result = await actions.updateClient(editingClientId, form);
      if (result.success) { toast.success('Client updated'); setShowDialog(false); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createClient(form);
      if (result.success) { toast.success('Client created'); setShowDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDelete = async () => {
    if (!deleteClientId) return;
    setSubmitting(true);
    const result = await actions.deleteClient(deleteClientId);
    if (result.success) toast.success('Client deleted');
    else toast.error(result.error.message);
    setDeleteClientId(null);
    setSubmitting(false);
  };

  const handleToggleActive = async (id: string, isActive: boolean) => {
    const result = await actions.updateClient(id, { isActive });
    if (result.success) toast.success(`Client ${isActive ? 'activated' : 'deactivated'}`);
    else toast.error(result.error.message);
  };

  const formatDate = (d: string) => new Date(d).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl text-foreground mb-2">Client Management</h1>
          <p className="text-muted-foreground">Manage clients for change tracking</p>
        </div>
        <Button onClick={handleNew}><Plus className="w-4 h-4 mr-2" />New Client</Button>
      </div>

      {clients.length === 0 ? (
        <EmptyState icon={Users} title="No clients yet" description="Create your first client to track changes by client" actionLabel="Create Client" onAction={handleNew} />
      ) : (
        <Card>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-border">
                  <th className="text-left px-4 py-3 text-sm text-foreground font-medium">Status</th>
                  <th className="text-left px-4 py-3 text-sm text-foreground font-medium">Name</th>
                  <th className="text-left px-4 py-3 text-sm text-foreground font-medium">Code</th>
                  <th className="text-left px-4 py-3 text-sm text-foreground font-medium">Description</th>
                  <th className="text-left px-4 py-3 text-sm text-foreground font-medium">Created</th>
                  <th className="text-right px-4 py-3 text-sm text-foreground font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {clients.map(client => (
                  <tr key={client.id} className="border-b border-border transition-colors hover:bg-muted/50">
                    <td className="px-4 py-3">
                      <Switch checked={client.isActive} onCheckedChange={(checked) => handleToggleActive(client.id, checked)} />
                    </td>
                    <td className="px-4 py-3">
                      <span className={client.isActive ? 'text-foreground' : 'text-muted-foreground'}>{client.name}</span>
                    </td>
                    <td className="px-4 py-3"><Badge variant="secondary">{client.code}</Badge></td>
                    <td className="px-4 py-3 text-sm text-muted-foreground">{client.description || '-'}</td>
                    <td className="px-4 py-3 text-sm text-muted-foreground">{formatDate(client.createdAt)}</td>
                    <td className="px-4 py-3 text-right">
                      <div className="flex justify-end gap-1">
                        <Button variant="ghost" size="sm" onClick={() => handleEdit(client.id)}><Pencil className="w-3 h-3" /></Button>
                        <Button variant="ghost" size="sm" onClick={() => setDeleteClientId(client.id)} className="text-destructive"><Trash2 className="w-3 h-3" /></Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      )}

      <Dialog open={showDialog} onOpenChange={setShowDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingClientId ? 'Edit Client' : 'New Client'}</DialogTitle>
            <DialogDescription>Enter the client details below.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Name</Label><Input value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} placeholder="Client name" /></div>
            <div><Label>Code</Label><Input value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value.toUpperCase() }))} placeholder="e.g. ACME" /></div>
            <div><Label>Description</Label><Textarea value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} placeholder="Optional description" /></div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowDialog(false)}>Cancel</Button>
            <Button onClick={handleSave} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingClientId ? 'Update' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteClientId} onOpenChange={() => setDeleteClientId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Client</AlertDialogTitle>
            <AlertDialogDescription>This will permanently delete this client. Changes associated with this client will have their client reference removed.</AlertDialogDescription>
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