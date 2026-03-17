import { createBrowserRouter, Navigate } from 'react-router';
import { Root } from '../components/Root';
import { WhatsNew } from '../components/WhatsNew';
import { ReleaseManagement } from '../components/ReleaseManagement';
import { TagManagement } from '../components/TagManagement';
import { ClientManagement } from '../components/ClientManagement';
import { ImportExport } from '../components/ImportExport';
import { IntegrationSetup } from '../components/IntegrationSetup';
import { AnalyticsDashboard } from '../components/AnalyticsDashboard';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { NotFound } from '../components/NotFound';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Root />,
    children: [
      {
        index: true,
        element: <WhatsNew />,
      },
      {
        path: 'admin',
        element: <Navigate to="/admin/releases" replace />,
      },
      {
        path: 'admin/analytics',
        element: <ProtectedRoute><AnalyticsDashboard /></ProtectedRoute>,
      },
      {
        path: 'admin/releases',
        element: <ProtectedRoute><ReleaseManagement /></ProtectedRoute>,
      },
      {
        path: 'admin/tags',
        element: <ProtectedRoute><TagManagement /></ProtectedRoute>,
      },
      {
        path: 'admin/clients',
        element: <ProtectedRoute><ClientManagement /></ProtectedRoute>,
      },
      {
        path: 'admin/import-export',
        element: <ProtectedRoute><ImportExport /></ProtectedRoute>,
      },
      {
        path: 'admin/integrations',
        element: <ProtectedRoute><IntegrationSetup /></ProtectedRoute>,
      },
      {
        path: '*',
        element: <NotFound />,
      },
    ],
  },
]);