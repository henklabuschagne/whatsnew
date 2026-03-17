export interface Client {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateClientData {
  name: string;
  code: string;
  description?: string;
}

export interface UpdateClientData {
  name?: string;
  code?: string;
  description?: string;
  isActive?: boolean;
}
