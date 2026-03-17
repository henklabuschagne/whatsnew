import { useState } from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { UserCircle2, Loader2, Shield, Eye } from 'lucide-react';
import { Alert, AlertDescription } from './ui/alert';
import { useAppStore } from '../hooks/useAppStore';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from './ui/card';
import type { PublicUser } from '../types';

interface LoginPageProps {
  onLogin: (user: PublicUser) => void;
}

export function LoginPage({ onLogin }: LoginPageProps) {
  const { reads, actions } = useAppStore('auth');
  const [useApiAuth, setUseApiAuth] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const users = reads.getUsers();

  const handleApiLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    const result = await actions.login(username, password);
    setLoading(false);
    if (result.success) {
      onLogin(result.data);
    } else {
      setError(result.error.message);
    }
  };

  const handleMockLogin = async (user: PublicUser) => {
    const result = await actions.loginAs(user);
    if (result.success) {
      onLogin(result.data);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-brand-primary-light to-brand-secondary-light flex items-center justify-center p-4">
      {useApiAuth ? (
        /* API Login - single card */
        <div className="max-w-md w-full">
          <Card>
            <CardHeader className="text-center">
              <div className="mx-auto mb-4 w-16 h-16 bg-brand-primary-light rounded-full flex items-center justify-center">
                <UserCircle2 className="w-8 h-8 text-brand-primary" />
              </div>
              <CardTitle>Welcome to What's New</CardTitle>
              <CardDescription>Sign in to continue</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleApiLogin} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="username">Username</Label>
                  <Input id="username" type="text" placeholder="admin or viewer" value={username} onChange={(e) => setUsername(e.target.value)} required />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="password">Password</Label>
                  <Input id="password" type="password" placeholder="Enter your password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                {error && (
                  <Alert variant="destructive">
                    <AlertDescription>{error}</AlertDescription>
                  </Alert>
                )}
                <Button type="submit" className="w-full" disabled={loading}>
                  {loading ? (<><Loader2 className="w-4 h-4 mr-2 animate-spin" />Signing in...</>) : 'Sign In'}
                </Button>
              </form>

              <div className="mt-6 p-4 bg-brand-primary-light rounded-lg border border-brand-secondary">
                <p className="text-sm font-medium text-brand-main mb-2">Demo Credentials:</p>
                <div className="text-xs text-brand-primary space-y-1">
                  <p>Admin: <span className="font-medium">admin</span> / <span className="font-medium">admin123</span></p>
                  <p>Viewer: <span className="font-medium">viewer</span> / <span className="font-medium">viewer123</span></p>
                </div>
              </div>

              <div className="mt-4 text-center">
                <button
                  onClick={() => setUseApiAuth(false)}
                  className="text-sm text-brand-primary hover:underline"
                >
                  Switch to role selection
                </button>
              </div>
            </CardContent>
          </Card>
        </div>
      ) : (
        /* Role Selection - two cards */
        <div className="max-w-3xl w-full">
          <div className="text-center mb-8">
            <h1 className="text-4xl font-bold text-brand-main mb-2">What's New</h1>
            <p className="text-muted-foreground">Select a role to get started</p>
          </div>

          <div className="grid md:grid-cols-2 gap-6">
            {users.map(user => (
              <Card
                key={user.id}
                className="cursor-pointer hover:shadow-lg transition-shadow border-2 hover:border-brand-primary"
                onClick={() => handleMockLogin(user)}
              >
                <CardHeader className="text-center">
                  <div className="mx-auto mb-4 w-16 h-16 bg-brand-primary-light rounded-full flex items-center justify-center">
                    {user.role === 'admin'
                      ? <Shield className="w-8 h-8 text-brand-primary" />
                      : <Eye className="w-8 h-8 text-brand-primary" />
                    }
                  </div>
                  <CardTitle>{user.name}</CardTitle>
                  <CardDescription className="capitalize">{user.role === 'admin' ? 'Full Access' : 'View Only'}</CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="text-sm text-muted-foreground space-y-2 mb-6">
                    {user.role === 'admin' ? (
                      <>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />Manage releases & changes</li>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />Tag & client management</li>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />Analytics dashboard</li>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />Import / Export & Integrations</li>
                      </>
                    ) : (
                      <>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />View published releases</li>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-brand-success" />Search & filter changes</li>
                        <li className="flex items-center gap-2"><span className="w-1.5 h-1.5 rounded-full bg-muted-foreground" />Read-only access</li>
                      </>
                    )}
                  </ul>
                  <Button className="w-full">Login as {user.name}</Button>
                </CardContent>
              </Card>
            ))}
          </div>

          <div className="text-center mt-6">
            <button
              onClick={() => setUseApiAuth(true)}
              className="text-sm text-brand-primary hover:underline"
            >
              Switch to API login
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
