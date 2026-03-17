import { useState } from 'react';
import type { ChangeType } from '../types';
import { Button } from './ui/button';
import { Textarea } from './ui/textarea';
import { Label } from './ui/label';
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "./ui/dialog";
import { FileSpreadsheet, Database, AlertCircle } from 'lucide-react';
import { Alert, AlertDescription } from './ui/alert';

interface ImportedChange {
  description: string;
  changeType: ChangeType;
  moduleTags: string[];
  title?: string;
}

interface ImportedRelease {
  version: string;
  releaseDate: string;
  title?: string;
  description?: string;
  changes: ImportedChange[];
}

interface ImportModalProps {
  open: boolean;
  onClose: () => void;
  onImport: (releases: ImportedRelease[]) => void;
}

export function ImportModal({ open, onClose, onImport }: ImportModalProps) {
  const [excelData, setExcelData] = useState('');
  const [sqlData, setSqlData] = useState('');
  const [error, setError] = useState('');

  const handleExcelImport = () => {
    try {
      setError('');
      const lines = excelData.trim().split('\n');
      if (lines.length === 0) {
        setError('No data to import');
        return;
      }

      const dataLines = lines.slice(1);
      const releasesMap = new Map<string, ImportedRelease>();

      dataLines.forEach((line, index) => {
        const parts = line.split(',').map(s => s.trim());
        if (parts.length < 5) {
          throw new Error(`Invalid format on line ${index + 2}`);
        }

        const [version, releaseDate, changeType, moduleTags, ...descParts] = parts;
        const description = descParts.join(',');

        if (!releasesMap.has(version)) {
          releasesMap.set(version, {
            version,
            releaseDate,
            title: `Release ${version}`,
            description: '',
            changes: [],
          });
        }

        const release = releasesMap.get(version)!;
        release.changes.push({
          title: description.slice(0, 80),
          description,
          changeType: changeType as ChangeType,
          moduleTags: moduleTags.split('|').filter(t => t),
        });
      });

      const releases = Array.from(releasesMap.values());
      onImport(releases);
      setExcelData('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to parse Excel data');
    }
  };

  const handleSqlImport = () => {
    try {
      setError('');
      const parsed = JSON.parse(sqlData);
      
      if (!Array.isArray(parsed)) {
        setError('SQL data must be an array of release objects');
        return;
      }

      const releases: ImportedRelease[] = parsed.map((item: any) => ({
        version: item.version,
        releaseDate: item.releaseDate || item.release_date,
        title: item.title || `Release ${item.version}`,
        description: item.description || '',
        changes: Array.isArray(item.changes) ? item.changes.map((change: any) => ({
          title: change.title || change.description?.slice(0, 80) || '',
          description: change.description,
          changeType: change.changeType || change.change_type,
          moduleTags: Array.isArray(change.moduleTags) ? change.moduleTags : (change.module_tags || []),
        })) : [],
      }));

      onImport(releases);
      setSqlData('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to parse SQL data');
    }
  };

  const handleClose = () => {
    setExcelData('');
    setSqlData('');
    setError('');
    onClose();
  };

  const excelExample = `Version,ReleaseDate,ChangeType,ModuleTags,Description
2.1.0,2024-01-15,new-feature,dashboard|reports,Added new analytics dashboard
2.1.0,2024-01-15,bug-fix,security,Fixed authentication timeout issue
2.0.5,2024-01-10,enhancement,import|export,Improved data import performance`;

  const sqlExample = `[
  {
    "version": "2.1.0",
    "releaseDate": "2024-01-15",
    "title": "Analytics Update",
    "changes": [
      {
        "title": "New analytics dashboard",
        "description": "Added new analytics dashboard",
        "changeType": "new-feature",
        "moduleTags": ["dashboard", "reports"]
      }
    ]
  }
]`;

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-3xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Import Releases</DialogTitle>
          <DialogDescription>
            Import release data from Excel (CSV) or SQL query results (JSON)
          </DialogDescription>
        </DialogHeader>

        <Tabs defaultValue="excel" className="w-full">
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="excel">
              <FileSpreadsheet className="w-4 h-4 mr-2" />
              Excel / CSV
            </TabsTrigger>
            <TabsTrigger value="sql">
              <Database className="w-4 h-4 mr-2" />
              SQL / JSON
            </TabsTrigger>
          </TabsList>

          <TabsContent value="excel" className="space-y-4">
            <div className="space-y-2">
              <Label>CSV Data</Label>
              <Textarea
                placeholder="Paste your CSV data here..."
                value={excelData}
                onChange={(e) => setExcelData(e.target.value)}
                className="font-mono h-48"
              />
            </div>

            <div className="bg-gray-50 p-4 rounded-lg space-y-2">
              <p className="text-gray-700">Expected format:</p>
              <pre className="text-gray-600 text-sm overflow-x-auto">
                {excelExample}
              </pre>
            </div>

            {error && (
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}

            <DialogFooter>
              <Button variant="outline" onClick={handleClose}>
                Cancel
              </Button>
              <Button onClick={handleExcelImport}>Import CSV</Button>
            </DialogFooter>
          </TabsContent>

          <TabsContent value="sql" className="space-y-4">
            <div className="space-y-2">
              <Label>JSON Data</Label>
              <Textarea
                placeholder="Paste your SQL query results as JSON here..."
                value={sqlData}
                onChange={(e) => setSqlData(e.target.value)}
                className="font-mono h-48"
              />
            </div>

            <div className="bg-gray-50 p-4 rounded-lg space-y-2">
              <p className="text-gray-700">Expected format:</p>
              <pre className="text-gray-600 text-sm overflow-x-auto">
                {sqlExample}
              </pre>
            </div>

            {error && (
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}

            <DialogFooter>
              <Button variant="outline" onClick={handleClose}>
                Cancel
              </Button>
              <Button onClick={handleSqlImport}>Import JSON</Button>
            </DialogFooter>
          </TabsContent>
        </Tabs>
      </DialogContent>
    </Dialog>
  );
}
