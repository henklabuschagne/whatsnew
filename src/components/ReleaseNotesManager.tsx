import { useState, useRef } from 'react';
import { Upload, Download, Trash2, File, Loader2, FileText, AlertCircle } from 'lucide-react';
import { Button } from './ui/button';
import { toast } from "sonner@2.0.3";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "./ui/alert-dialog";

interface ReleaseNote {
  releaseNoteId: string;
  changeId: string;
  fileName: string;
  fileSize: number;
  fileType: string;
  fileExtension: string;
  uploadedBy?: string;
  uploadedByName?: string;
  uploadedAt: string;
}

interface ReleaseNotesManagerProps {
  changeId: string;
  readOnly?: boolean;
}

// Mock data for release notes (simulates API responses)
const mockReleaseNotes: Record<string, ReleaseNote[]> = {};

export function ReleaseNotesManager({ changeId, readOnly = false }: ReleaseNotesManagerProps) {
  const [releaseNotes, setReleaseNotes] = useState<ReleaseNote[]>(mockReleaseNotes[changeId] || []);
  const [uploading, setUploading] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      handleUpload(file);
    }
  };

  const handleUpload = async (file: File) => {
    const maxSize = 50 * 1024 * 1024;
    if (file.size > maxSize) {
      toast.error('File size exceeds 50MB limit');
      return;
    }

    const allowedExtensions = ['.pdf', '.doc', '.docx', '.txt', '.md', '.png', '.jpg', '.jpeg', '.gif', '.xlsx', '.xls', '.pptx', '.ppt'];
    const extension = '.' + file.name.split('.').pop()?.toLowerCase();
    if (!allowedExtensions.includes(extension)) {
      toast.error(`File type not allowed. Allowed: ${allowedExtensions.join(', ')}`);
      return;
    }

    setUploading(true);
    // Simulate upload latency
    await new Promise(r => setTimeout(r, 500));

    const newNote: ReleaseNote = {
      releaseNoteId: crypto.randomUUID(),
      changeId,
      fileName: file.name,
      fileSize: file.size,
      fileType: file.type,
      fileExtension: extension,
      uploadedBy: 'current-user',
      uploadedByName: 'Admin User',
      uploadedAt: new Date().toISOString(),
    };

    const updated = [...releaseNotes, newNote];
    setReleaseNotes(updated);
    mockReleaseNotes[changeId] = updated;
    toast.success('File uploaded successfully');

    setUploading(false);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const handleDownload = (releaseNote: ReleaseNote) => {
    // In mock mode, create a dummy download
    const blob = new Blob([`Mock content for ${releaseNote.fileName}`], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = releaseNote.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
    toast.success('File downloaded');
  };

  const confirmDelete = () => {
    if (!deleteId) return;
    const updated = releaseNotes.filter(n => n.releaseNoteId !== deleteId);
    setReleaseNotes(updated);
    mockReleaseNotes[changeId] = updated;
    toast.success('Release note deleted');
    setDeleteId(null);
  };

  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', month: 'short', day: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  };

  const getFileIcon = (fileExtension: string) => {
    const ext = fileExtension.toLowerCase();
    if (['.pdf'].includes(ext)) return <FileText className="w-5 h-5 text-red-600" />;
    if (['.doc', '.docx'].includes(ext)) return <FileText className="w-5 h-5 text-blue-600" />;
    if (['.txt', '.md'].includes(ext)) return <FileText className="w-5 h-5 text-gray-600" />;
    if (['.png', '.jpg', '.jpeg', '.gif'].includes(ext)) return <File className="w-5 h-5 text-green-600" />;
    if (['.xlsx', '.xls'].includes(ext)) return <FileText className="w-5 h-5 text-green-700" />;
    if (['.pptx', '.ppt'].includes(ext)) return <FileText className="w-5 h-5 text-orange-600" />;
    return <File className="w-5 h-5 text-gray-600" />;
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-gray-900">Release Notes & Attachments</h3>
          <p className="text-gray-600 text-sm">Upload documentation, images, or related files</p>
        </div>
        {!readOnly && (
          <Button onClick={() => fileInputRef.current?.click()} disabled={uploading} size="sm">
            {uploading ? (
              <><Loader2 className="w-4 h-4 mr-2 animate-spin" />Uploading...</>
            ) : (
              <><Upload className="w-4 h-4 mr-2" />Upload File</>
            )}
          </Button>
        )}
        <input
          ref={fileInputRef}
          type="file"
          className="hidden"
          onChange={handleFileSelect}
          accept=".pdf,.doc,.docx,.txt,.md,.png,.jpg,.jpeg,.gif,.xlsx,.xls,.pptx,.ppt"
        />
      </div>

      {!readOnly && (
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 flex items-start gap-2">
          <AlertCircle className="w-4 h-4 text-blue-600 mt-0.5 flex-shrink-0" />
          <div className="text-sm text-blue-900">
            <p>Supported: Documents (PDF, Word, Text), Images (PNG, JPG, GIF), Spreadsheets (Excel), Presentations (PowerPoint). Max 50MB.</p>
          </div>
        </div>
      )}

      {releaseNotes.length > 0 ? (
        <div className="space-y-2">
          {releaseNotes.map(note => (
            <div key={note.releaseNoteId} className="flex items-center justify-between p-4 bg-white border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
              <div className="flex items-center gap-3 flex-1 min-w-0">
                {getFileIcon(note.fileExtension)}
                <div className="flex-1 min-w-0">
                  <p className="text-gray-900 truncate">{note.fileName}</p>
                  <div className="flex items-center gap-3 text-sm text-gray-600">
                    <span>{formatFileSize(note.fileSize)}</span>
                    <span>{formatDate(note.uploadedAt)}</span>
                    {note.uploadedByName && <span>by {note.uploadedByName}</span>}
                  </div>
                </div>
              </div>
              <div className="flex items-center gap-2 ml-4">
                <Button variant="ghost" size="sm" onClick={() => handleDownload(note)} title="Download">
                  <Download className="w-4 h-4" />
                </Button>
                {!readOnly && (
                  <Button variant="ghost" size="sm" onClick={() => setDeleteId(note.releaseNoteId)} title="Delete">
                    <Trash2 className="w-4 h-4 text-red-600" />
                  </Button>
                )}
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="text-center py-8 bg-gray-50 border border-gray-200 rounded-lg">
          <File className="w-12 h-12 text-gray-400 mx-auto mb-2" />
          <p className="text-gray-600">No release notes attached</p>
          {!readOnly && <p className="text-gray-500 text-sm mt-1">Upload files to attach documentation</p>}
        </div>
      )}

      <AlertDialog open={deleteId !== null} onOpenChange={() => setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Release Note</AlertDialogTitle>
            <AlertDialogDescription>Are you sure? This action cannot be undone.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete}>Delete</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
