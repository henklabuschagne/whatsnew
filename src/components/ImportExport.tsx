import { useState } from 'react';
import { useAppStore } from '../hooks/useAppStore';
import { Upload, Download, FileSpreadsheet, Loader2 } from 'lucide-react';
import { Button } from './ui/button';
import { Card } from './ui/card';
import { toast } from "sonner@2.0.3";

export function ImportExport() {
  const { actions } = useAppStore('releases', 'changes');
  const [importing, setImporting] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [downloading, setDownloading] = useState(false);

  const handleImport = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setImporting(true);
    const result = await actions.importExcel(file);
    setImporting(false);
    if (result.success) {
      toast.success(`Import complete: ${result.data.importedReleases} releases, ${result.data.importedChanges} changes`);
    } else {
      toast.error(result.error.message);
    }
    e.target.value = '';
  };

  const handleExport = async () => {
    setExporting(true);
    const result = await actions.exportExcel();
    setExporting(false);
    if (result.success) {
      const url = URL.createObjectURL(result.data);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'releases-export.xlsx';
      a.click();
      URL.revokeObjectURL(url);
      toast.success('Export downloaded');
    } else {
      toast.error(result.error.message);
    }
  };

  const handleTemplate = async () => {
    setDownloading(true);
    const result = await actions.downloadTemplate();
    setDownloading(false);
    if (result.success) {
      const url = URL.createObjectURL(result.data);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'import-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
      toast.success('Template downloaded');
    } else {
      toast.error(result.error.message);
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl text-foreground mb-2">Import & Export</h1>
        <p className="text-muted-foreground">Import data from Excel or export your releases</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card className="p-6">
          <div className="flex flex-col items-center text-center gap-4">
            <div className="p-3 bg-brand-success-light rounded-lg">
              <Upload className="w-6 h-6 text-brand-success" />
            </div>
            <div>
              <h3 className="text-foreground mb-1">Import from Excel</h3>
              <p className="text-sm text-muted-foreground">Upload an Excel file to import releases and changes</p>
            </div>
            <label className="cursor-pointer">
              <input type="file" accept=".xlsx,.xls" onChange={handleImport} className="hidden" />
              <Button disabled={importing} asChild>
                <span>
                  {importing ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : <Upload className="w-4 h-4 mr-2" />}
                  {importing ? 'Importing...' : 'Select File'}
                </span>
              </Button>
            </label>
          </div>
        </Card>

        <Card className="p-6">
          <div className="flex flex-col items-center text-center gap-4">
            <div className="p-3 bg-brand-primary-light rounded-lg">
              <Download className="w-6 h-6 text-brand-primary" />
            </div>
            <div>
              <h3 className="text-foreground mb-1">Export to Excel</h3>
              <p className="text-sm text-muted-foreground">Download all releases and changes as an Excel file</p>
            </div>
            <Button onClick={handleExport} disabled={exporting}>
              {exporting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : <Download className="w-4 h-4 mr-2" />}
              {exporting ? 'Exporting...' : 'Export Data'}
            </Button>
          </div>
        </Card>

        <Card className="p-6">
          <div className="flex flex-col items-center text-center gap-4">
            <div className="p-3 bg-brand-warning-light rounded-lg">
              <FileSpreadsheet className="w-6 h-6 text-brand-warning" />
            </div>
            <div>
              <h3 className="text-foreground mb-1">Download Template</h3>
              <p className="text-sm text-muted-foreground">Get the Excel template for importing data</p>
            </div>
            <Button variant="outline" onClick={handleTemplate} disabled={downloading}>
              {downloading ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : <FileSpreadsheet className="w-4 h-4 mr-2" />}
              {downloading ? 'Downloading...' : 'Get Template'}
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
}