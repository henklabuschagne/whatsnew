import { useState } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Plus, Pencil, Trash2, Loader2, Database, Play, CheckCircle, XCircle } from 'lucide-react';
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';

export function IntegrationSetup() {
  const { sqlConnections, sqlQueries, actions } = useAppStore('connections', 'queries');

  // Connection state
  const [showConnDialog, setShowConnDialog] = useState(false);
  const [editingConnId, setEditingConnId] = useState<string | null>(null);
  const [connForm, setConnForm] = useState({ name: '', server: '', database: '', username: '', password: '', useIntegratedSecurity: true, isActive: true });
  const [deleteConnId, setDeleteConnId] = useState<string | null>(null);

  // Query state
  const [showQueryDialog, setShowQueryDialog] = useState(false);
  const [editingQueryId, setEditingQueryId] = useState<string | null>(null);
  const [queryForm, setQueryForm] = useState({ connectionId: '', name: '', description: '', queryText: '', isActive: true });
  const [deleteQueryId, setDeleteQueryId] = useState<string | null>(null);

  const [submitting, setSubmitting] = useState(false);
  const [testing, setTesting] = useState(false);

  // Connection CRUD
  const handleNewConn = () => {
    setEditingConnId(null);
    setConnForm({ name: '', server: '', database: '', username: '', password: '', useIntegratedSecurity: true, isActive: true });
    setShowConnDialog(true);
  };

  const handleEditConn = (id: string) => {
    const c = sqlConnections.find(x => x.id === id);
    if (!c) return;
    setEditingConnId(id);
    setConnForm({ name: c.name, server: c.server, database: c.database, username: c.username || '', password: '', useIntegratedSecurity: c.useIntegratedSecurity, isActive: c.isActive });
    setShowConnDialog(true);
  };

  const handleSaveConn = async () => {
    if (!connForm.name.trim() || !connForm.server.trim() || !connForm.database.trim()) { toast.error('Name, server, and database are required'); return; }
    setSubmitting(true);
    if (editingConnId) {
      const result = await actions.updateConnection(editingConnId, connForm);
      if (result.success) { toast.success('Connection updated'); setShowConnDialog(false); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createConnection(connForm);
      if (result.success) { toast.success('Connection created'); setShowConnDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDeleteConn = async () => {
    if (!deleteConnId) return;
    setSubmitting(true);
    const result = await actions.deleteConnection(deleteConnId);
    if (result.success) toast.success('Connection deleted');
    else toast.error(result.error.message);
    setDeleteConnId(null);
    setSubmitting(false);
  };

  const handleTestConn = async () => {
    setTesting(true);
    const result = await actions.testConnection({ server: connForm.server, database: connForm.database, username: connForm.username, password: connForm.password, useIntegratedSecurity: connForm.useIntegratedSecurity });
    setTesting(false);
    if (result.success) toast.success('Connection test successful');
    else toast.error(result.error.message);
  };

  // Query CRUD
  const handleNewQuery = () => {
    setEditingQueryId(null);
    setQueryForm({ connectionId: sqlConnections[0]?.id || '', name: '', description: '', queryText: '', isActive: true });
    setShowQueryDialog(true);
  };

  const handleEditQuery = (id: string) => {
    const q = sqlQueries.find(x => x.id === id);
    if (!q) return;
    setEditingQueryId(id);
    setQueryForm({ connectionId: q.connectionId, name: q.name, description: q.description || '', queryText: q.queryText, isActive: q.isActive });
    setShowQueryDialog(true);
  };

  const handleSaveQuery = async () => {
    if (!queryForm.name.trim() || !queryForm.queryText.trim()) { toast.error('Name and query text are required'); return; }
    setSubmitting(true);
    if (editingQueryId) {
      const result = await actions.updateQuery(editingQueryId, queryForm);
      if (result.success) { toast.success('Query updated'); setShowQueryDialog(false); }
      else toast.error(result.error.message);
    } else {
      const result = await actions.createQuery(queryForm);
      if (result.success) { toast.success('Query created'); setShowQueryDialog(false); }
      else toast.error(result.error.message);
    }
    setSubmitting(false);
  };

  const handleDeleteQuery = async () => {
    if (!deleteQueryId) return;
    setSubmitting(true);
    const result = await actions.deleteQuery(deleteQueryId);
    if (result.success) toast.success('Query deleted');
    else toast.error(result.error.message);
    setDeleteQueryId(null);
    setSubmitting(false);
  };

  const handleExecuteQuery = async (id: string) => {
    const result = await actions.executeQuery(id);
    if (result.success) toast.success(result.data.message);
    else toast.error(result.error.message);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl text-foreground mb-2">SQL Integration</h1>
        <p className="text-muted-foreground">Manage database connections and queries</p>
      </div>

      <Tabs defaultValue="connections">
        <TabsList>
          <TabsTrigger value="connections">Connections ({sqlConnections.length})</TabsTrigger>
          <TabsTrigger value="queries">Queries ({sqlQueries.length})</TabsTrigger>
        </TabsList>

        <TabsContent value="connections" className="mt-4">
          <div className="flex justify-end mb-4">
            <Button onClick={handleNewConn}><Plus className="w-4 h-4 mr-2" />New Connection</Button>
          </div>
          {sqlConnections.length === 0 ? (
            <EmptyState icon={Database} title="No connections" description="Create a database connection to get started" actionLabel="New Connection" onAction={handleNewConn} />
          ) : (
            <div className="space-y-3">
              {sqlConnections.map(conn => (
                <div key={conn.id} className="bg-card rounded-xl border border-border p-4 flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <Database className={`w-5 h-5 ${conn.isActive ? 'text-brand-success' : 'text-muted-foreground'}`} />
                    <div>
                      <div className="text-foreground text-sm">{conn.name}</div>
                      <div className="text-muted-foreground text-xs">{conn.server} / {conn.database}</div>
                    </div>
                    {conn.isActive
                      ? <Badge variant="outline" className="text-brand-success border-brand-success-mid bg-brand-success-light">Active</Badge>
                      : <Badge variant="secondary">Inactive</Badge>
                    }
                    {conn.useIntegratedSecurity && <Badge variant="secondary" className="text-xs">Windows Auth</Badge>}
                  </div>
                  <div className="flex gap-1">
                    <Button variant="ghost" size="sm" onClick={() => handleEditConn(conn.id)}><Pencil className="w-3 h-3" /></Button>
                    <Button variant="ghost" size="sm" onClick={() => setDeleteConnId(conn.id)} className="text-red-600"><Trash2 className="w-3 h-3" /></Button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </TabsContent>

        <TabsContent value="queries" className="mt-4">
          <div className="flex justify-end mb-4">
            <Button onClick={handleNewQuery} disabled={sqlConnections.length === 0}><Plus className="w-4 h-4 mr-2" />New Query</Button>
          </div>
          {sqlQueries.length === 0 ? (
            <EmptyState icon={Play} title="No queries" description={sqlConnections.length === 0 ? "Create a connection first, then add queries" : "Create a query to run against your connections"} actionLabel={sqlConnections.length > 0 ? "New Query" : undefined} onAction={sqlConnections.length > 0 ? handleNewQuery : undefined} />
          ) : (
            <div className="space-y-3">
              {sqlQueries.map(query => {
                const conn = sqlConnections.find(c => c.id === query.connectionId);
                return (
                  <div key={query.id} className="bg-card rounded-xl border border-border p-4">
                    <div className="flex items-center justify-between mb-2">
                      <div>
                        <div className="text-foreground text-sm">{query.name}</div>
                        <div className="text-muted-foreground text-xs">{query.description || 'No description'} · Connection: {conn?.name || 'Unknown'}</div>
                      </div>
                      <div className="flex gap-1">
                        <Button variant="outline" size="sm" onClick={() => handleExecuteQuery(query.id)}><Play className="w-3 h-3 mr-1" />Run</Button>
                        <Button variant="ghost" size="sm" onClick={() => handleEditQuery(query.id)}><Pencil className="w-3 h-3" /></Button>
                        <Button variant="ghost" size="sm" onClick={() => setDeleteQueryId(query.id)} className="text-red-600"><Trash2 className="w-3 h-3" /></Button>
                      </div>
                    </div>
                    <pre className="bg-muted p-2 rounded text-xs text-foreground overflow-x-auto">{query.queryText}</pre>
                  </div>
                );
              })}
            </div>
          )}
        </TabsContent>
      </Tabs>

      {/* Connection Dialog */}
      <Dialog open={showConnDialog} onOpenChange={setShowConnDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingConnId ? 'Edit Connection' : 'New Connection'}</DialogTitle>
            <DialogDescription>Configure your SQL Server connection.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Name</Label><Input value={connForm.name} onChange={e => setConnForm(f => ({ ...f, name: e.target.value }))} placeholder="Connection name" /></div>
            <div><Label>Server</Label><Input value={connForm.server} onChange={e => setConnForm(f => ({ ...f, server: e.target.value }))} placeholder="server-name.domain" /></div>
            <div><Label>Database</Label><Input value={connForm.database} onChange={e => setConnForm(f => ({ ...f, database: e.target.value }))} placeholder="DatabaseName" /></div>
            <div className="flex items-center gap-2">
              <Switch checked={connForm.useIntegratedSecurity} onCheckedChange={v => setConnForm(f => ({ ...f, useIntegratedSecurity: v }))} />
              <Label>Use Windows Authentication</Label>
            </div>
            {!connForm.useIntegratedSecurity && (
              <div className="grid grid-cols-2 gap-4">
                <div><Label>Username</Label><Input value={connForm.username} onChange={e => setConnForm(f => ({ ...f, username: e.target.value }))} /></div>
                <div><Label>Password</Label><Input type="password" value={connForm.password} onChange={e => setConnForm(f => ({ ...f, password: e.target.value }))} /></div>
              </div>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={handleTestConn} disabled={testing}>
              {testing ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : <CheckCircle className="w-4 h-4 mr-2" />}Test
            </Button>
            <Button variant="outline" onClick={() => setShowConnDialog(false)}>Cancel</Button>
            <Button onClick={handleSaveConn} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingConnId ? 'Update' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Query Dialog */}
      <Dialog open={showQueryDialog} onOpenChange={setShowQueryDialog}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>{editingQueryId ? 'Edit Query' : 'New Query'}</DialogTitle>
            <DialogDescription>Configure your SQL query.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div><Label>Name</Label><Input value={queryForm.name} onChange={e => setQueryForm(f => ({ ...f, name: e.target.value }))} placeholder="Query name" /></div>
            <div><Label>Description</Label><Input value={queryForm.description} onChange={e => setQueryForm(f => ({ ...f, description: e.target.value }))} placeholder="Optional description" /></div>
            <div>
              <Label>Connection</Label>
              <select className="w-full border border-gray-200 rounded-md px-3 py-2 text-sm" value={queryForm.connectionId} onChange={e => setQueryForm(f => ({ ...f, connectionId: e.target.value }))}>
                {sqlConnections.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div><Label>SQL Query</Label><Textarea value={queryForm.queryText} onChange={e => setQueryForm(f => ({ ...f, queryText: e.target.value }))} placeholder="SELECT * FROM ..." rows={6} className="font-mono text-sm" /></div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowQueryDialog(false)}>Cancel</Button>
            <Button onClick={handleSaveQuery} disabled={submitting}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              {editingQueryId ? 'Update' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirms */}
      <AlertDialog open={!!deleteConnId} onOpenChange={() => setDeleteConnId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader><AlertDialogTitle>Delete Connection</AlertDialogTitle><AlertDialogDescription>This will also delete all queries associated with this connection.</AlertDialogDescription></AlertDialogHeader>
          <AlertDialogFooter><AlertDialogCancel>Cancel</AlertDialogCancel><AlertDialogAction onClick={handleDeleteConn} className="bg-red-600 hover:bg-red-700">Delete</AlertDialogAction></AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <AlertDialog open={!!deleteQueryId} onOpenChange={() => setDeleteQueryId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader><AlertDialogTitle>Delete Query</AlertDialogTitle><AlertDialogDescription>This will permanently delete this query.</AlertDialogDescription></AlertDialogHeader>
          <AlertDialogFooter><AlertDialogCancel>Cancel</AlertDialogCancel><AlertDialogAction onClick={handleDeleteQuery} className="bg-red-600 hover:bg-red-700">Delete</AlertDialogAction></AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}