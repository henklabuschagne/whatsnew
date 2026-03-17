import axios, { AxiosInstance } from 'axios';
import { config } from '../utils/config';
import { mockData } from '../utils/mockData';

class ApiService {
  private api: AxiosInstance;
  private useMockData: boolean;

  constructor() {
    this.useMockData = config.enableMockData;
    
    this.api = axios.create({
      baseURL: config.apiBaseUrl,
      timeout: config.apiTimeout,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include JWT token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('auth_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add response interceptor to handle errors
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        // Network error - switch to mock data if enabled
        if (!error.response && this.useMockData) {
          return Promise.reject({ ...error, useMockData: true });
        }
        
        if (error.response?.status === 401) {
          // Unauthorized - clear token and redirect to login
          localStorage.removeItem('auth_token');
          localStorage.removeItem('whats-new-current-user');
          window.location.href = '/';
        }
        return Promise.reject(error);
      }
    );
  }

  // Helper to handle API calls with mock data fallback
  private async handleRequest<T>(
    apiCall: () => Promise<any>,
    mockDataFallback: () => T
  ): Promise<T> {
    if (this.useMockData) {
      try {
        const result = await apiCall();
        return result;
      } catch (error: any) {
        // Use mock data if:
        // 1. Network error (no response)
        // 2. Server not running (404, 500, 502, 503)
        // 3. Explicitly flagged to use mock data
        if (error.useMockData || !error.response || 
            error.response?.status === 404 || 
            error.response?.status >= 500) {
          console.log('🔄 API unavailable, using mock data');
          return mockDataFallback();
        }
        throw error;
      }
    }
    return await apiCall();
  }

  // Authentication
  async login(username: string, password: string) {
    return this.handleRequest(
      async () => {
        // Backend expects 'email' field, but we accept 'username' for flexibility
        const response = await this.api.post('/auth/login', { email: username, password });
        return response.data;
      },
      () => mockData.login(username, password)
    );
  }

  async getCurrentUser() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/auth/me');
        return response.data;
      },
      () => mockData.getCurrentUser()
    );
  }

  // No logout endpoint in backend - just clear token client-side
  logout() {
    this.clearAuthToken();
    localStorage.removeItem('whats-new-current-user');
    localStorage.removeItem('auth_token');
  }

  // Helper method to set auth token
  setAuthToken(token: string) {
    localStorage.setItem('auth_token', token);
  }

  // Helper method to clear auth token
  clearAuthToken() {
    localStorage.removeItem('auth_token');
  }

  // Tags
  async getAllTags(type?: string) {
    const params = type ? { type } : {};
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/tags', { params });
        return response.data.data; // Unwrap ApiResponse
      },
      () => mockData.getAllTags(type)
    );
  }

  async getTagById(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/tags/${id}`);
        return response.data.data; // Unwrap ApiResponse
      },
      () => mockData.getTagById(id)
    );
  }

  async createTag(tagData: { label: string; value: string; type: string }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/tags', tagData);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.createTag(tagData)
    );
  }

  async updateTag(id: string, tagData: { label: string; isActive?: boolean }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/tags/${id}`, tagData);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.updateTag(id, tagData)
    );
  }

  async deleteTag(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/tags/${id}`);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.deleteTag(id)
    );
  }

  // Releases
  async getAllReleases(includeChanges: boolean = true) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases', { params: { includeChanges } });
        return response.data;
      },
      () => mockData.getAllReleases(includeChanges)
    );
  }

  async getReleasesWithFilters(filters: {
    searchTerm?: string;
    changeType?: string;
    moduleTagId?: string;
    fromDate?: string;
    toDate?: string;
    includeChanges?: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases/filter', { params: filters });
        return response.data;
      },
      () => mockData.getReleasesWithFilters(filters)
    );
  }

  async getStatistics() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases/statistics');
        return response.data;
      },
      () => mockData.getStatistics()
    );
  }

  async getPopularTags(topN: number = 10) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases/popular-tags', { params: { topN } });
        return response.data;
      },
      () => mockData.getPopularTags(topN)
    );
  }

  async getVersionList() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases/versions');
        return response.data;
      },
      () => mockData.getVersionList()
    );
  }

  async searchChanges(q: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/releases/search', { params: { q } });
        return response.data;
      },
      () => mockData.searchChanges(q)
    );
  }

  async getReleaseById(id: string, includeChanges: boolean = true) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/releases/${id}`, { params: { includeChanges } });
        return response.data;
      },
      () => mockData.getReleaseById(id, includeChanges)
    );
  }

  async createRelease(releaseData: { version: string; releaseDate: string }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/releases', releaseData);
        return response.data;
      },
      () => mockData.createRelease(releaseData)
    );
  }

  async updateRelease(id: string, releaseData: { version: string; releaseDate: string }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/releases/${id}`, releaseData);
        return response.data;
      },
      () => mockData.updateRelease(id, releaseData)
    );
  }

  async deleteRelease(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/releases/${id}`);
        return response.data;
      },
      () => mockData.deleteRelease(id)
    );
  }

  // Changes
  async getChangesByReleaseId(releaseId: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/changes/release/${releaseId}`);
        return response.data;
      },
      () => mockData.getChangesByReleaseId(releaseId)
    );
  }

  async getChangeById(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/changes/${id}`);
        return response.data;
      },
      () => mockData.getChangeById(id)
    );
  }

  async createChange(changeData: { 
    releaseId: string; 
    description: string; 
    changeType: string; 
    tagIds: string[];
    clientId?: string;
    ticketNumber?: string;
    devopsNumber?: string;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/changes', changeData);
        return response.data;
      },
      () => mockData.createChange(changeData)
    );
  }

  async updateChange(id: string, changeData: { 
    description: string; 
    changeType: string; 
    tagIds: string[];
    clientId?: string;
    ticketNumber?: string;
    devopsNumber?: string;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/changes/${id}`, changeData);
        return response.data;
      },
      () => mockData.updateChange(id, changeData)
    );
  }

  async deleteChange(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/changes/${id}`);
        return response.data;
      },
      () => mockData.deleteChange(id)
    );
  }

  // Import/Export
  async importExcel(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/importexport/import/excel', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });
        return response.data;
      },
      () => mockData.importExcel(file)
    );
  }

  async exportExcel() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/importexport/export/excel', {
          responseType: 'blob',
        });
        return response.data;
      },
      () => mockData.exportExcel()
    );
  }

  async downloadTemplate() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/importexport/template/excel', {
          responseType: 'blob',
        });
        return response.data;
      },
      () => mockData.downloadTemplate()
    );
  }

  // SQL Integration - Connections
  async getAllConnections() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/sqlintegration/connections');
        return response.data;
      },
      () => mockData.getAllConnections()
    );
  }

  async getConnectionById(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/sqlintegration/connections/${id}`);
        return response.data;
      },
      () => mockData.getConnectionById(id)
    );
  }

  async createConnection(connectionData: {
    name: string;
    server: string;
    database: string;
    username?: string;
    password?: string;
    useIntegratedSecurity: boolean;
    isActive: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/sqlintegration/connections', connectionData);
        return response.data;
      },
      () => mockData.createConnection(connectionData)
    );
  }

  async updateConnection(id: string, connectionData: {
    name: string;
    server: string;
    database: string;
    username?: string;
    password?: string;
    useIntegratedSecurity: boolean;
    isActive: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/sqlintegration/connections/${id}`, connectionData);
        return response.data;
      },
      () => mockData.updateConnection(id, connectionData)
    );
  }

  async deleteConnection(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/sqlintegration/connections/${id}`);
        return response.data;
      },
      () => mockData.deleteConnection(id)
    );
  }

  async testConnection(connectionData: {
    server: string;
    database: string;
    username?: string;
    password?: string;
    useIntegratedSecurity: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/sqlintegration/connections/test', connectionData);
        return response.data;
      },
      () => mockData.testConnection(connectionData)
    );
  }

  // SQL Integration - Queries
  async getAllQueries() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/sqlintegration/queries');
        return response.data;
      },
      () => mockData.getAllQueries()
    );
  }

  async getQueryById(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/sqlintegration/queries/${id}`);
        return response.data;
      },
      () => mockData.getQueryById(id)
    );
  }

  async createQuery(queryData: {
    connectionId: string;
    name: string;
    description?: string;
    queryText: string;
    isActive: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/sqlintegration/queries', queryData);
        return response.data;
      },
      () => mockData.createQuery(queryData)
    );
  }

  async updateQuery(id: string, queryData: {
    name: string;
    description?: string;
    queryText: string;
    isActive: boolean;
  }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/sqlintegration/queries/${id}`, queryData);
        return response.data;
      },
      () => mockData.updateQuery(id, queryData)
    );
  }

  async deleteQuery(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/sqlintegration/queries/${id}`);
        return response.data;
      },
      () => mockData.deleteQuery(id)
    );
  }

  async executeQuery(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post(`/sqlintegration/queries/${id}/execute`);
        return response.data;
      },
      () => mockData.executeQuery(id)
    );
  }

  // Analytics
  async getAnalyticsTimeline(months: number = 12) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/timeline', { params: { months } });
        return response.data;
      },
      () => mockData.getAnalyticsTimeline(months)
    );
  }

  async getModuleDistribution() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/module-distribution');
        return response.data;
      },
      () => mockData.getModuleDistribution()
    );
  }

  async getChangeTypeDistribution() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/change-type-distribution');
        return response.data;
      },
      () => mockData.getChangeTypeDistribution()
    );
  }

  async getRecentActivity(topN: number = 20) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/recent-activity', { params: { topN } });
        return response.data;
      },
      () => mockData.getRecentActivity(topN)
    );
  }

  async getReleaseVelocity() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/release-velocity');
        return response.data;
      },
      () => mockData.getReleaseVelocity()
    );
  }

  async getTopReleases(topN: number = 10) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/top-releases', { params: { topN } });
        return response.data;
      },
      () => mockData.getTopReleases(topN)
    );
  }

  async getDashboardSummary() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/dashboard-summary');
        return response.data;
      },
      () => mockData.getDashboardSummary()
    );
  }

  async getChangeTrends(days: number = 30) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/change-trends', { params: { days } });
        return response.data;
      },
      () => mockData.getChangeTrends(days)
    );
  }

  async getClientDistribution() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/client-distribution');
        return response.data;
      },
      () => mockData.getClientDistribution()
    );
  }

  async getTimeToActionMetrics() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/analytics/time-to-action');
        return response.data;
      },
      () => mockData.getTimeToActionMetrics()
    );
  }

  // Clients
  async getAllClients() {
    return this.handleRequest(
      async () => {
        const response = await this.api.get('/clients');
        return response.data.data; // Unwrap ApiResponse
      },
      () => mockData.getAllClients()
    );
  }

  async getClientById(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/clients/${id}`);
        return response.data.data; // Unwrap ApiResponse
      },
      () => mockData.getClientById(id)
    );
  }

  async createClient(clientData: { name: string; code: string; description?: string }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.post('/clients', clientData);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.createClient(clientData)
    );
  }

  async updateClient(id: string, clientData: { name?: string; code?: string; description?: string; isActive?: boolean }) {
    return this.handleRequest(
      async () => {
        const response = await this.api.put(`/clients/${id}`, clientData);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.updateClient(id, clientData)
    );
  }

  async deleteClient(id: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/clients/${id}`);
        return response.data; // Returns full ApiResponse for success checking
      },
      () => mockData.deleteClient(id)
    );
  }

  // Release Notes
  async getReleaseNotesByChangeId(changeId: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/releasenotes/change/${changeId}`);
        return response.data.data; // Unwrap ApiResponse
      },
      () => [] // Return empty array as fallback
    );
  }

  async uploadReleaseNote(changeId: string, file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.handleRequest(
      async () => {
        const response = await this.api.post(`/releasenotes/change/${changeId}/upload`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });
        return response.data; // Returns full ApiResponse
      },
      () => ({ success: true, message: 'File uploaded (mock mode)' })
    );
  }

  async deleteReleaseNote(releaseNoteId: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.delete(`/releasenotes/${releaseNoteId}`);
        return response.data; // Returns full ApiResponse
      },
      () => ({ success: true, message: 'Release note deleted (mock mode)' })
    );
  }

  async downloadReleaseNote(releaseNoteId: string) {
    return this.handleRequest(
      async () => {
        const response = await this.api.get(`/releasenotes/${releaseNoteId}/download`, {
          responseType: 'blob',
        });
        return response.data;
      },
      () => new Blob(['Mock release note content'], { type: 'application/octet-stream' })
    );
  }
}

export const apiService = new ApiService();