export type UserRole = 'viewer' | 'admin';

export interface User {
  id: string;
  name: string;
  role: UserRole;
}
