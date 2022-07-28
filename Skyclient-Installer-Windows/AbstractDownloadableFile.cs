using System;
using System.IO;
using Skyclient.Utilities;
using System.Threading.Tasks;

namespace Skyclient
{
    public abstract class AbstractDownloadableFile
    {
        public bool CancelDownload = false;

        public event EventHandler DownloadStatusChanged;
        public virtual DownloadableFileStatus DownloadStatus
        {
            get { return _status; }
            set 
            {
                if (value != _status)
                {
                    var oldstatus = _status;
                    _status = value;
                    InvokeDownloadStatusChanged();
                }
            }
        }
        protected DownloadableFileStatus _status;

        public void InvokeDownloadStatusChanged()
        {
            DownloadStatusChanged?.Invoke(this, new EventArgs());
        }

        public abstract string FileDestination { get; }
        public abstract string FileSource { get; }
        public abstract string TempFileDestination { get; }

        public int Guid => (Path.GetFileName(FileDestination) + "|" + FileSource).GetHashCode();

        public Task<string?> Download()
        {
            return RepoUtils.DownloadTempFile(this);
        }
    }

    public enum DownloadableFileStatus
    {
        Idle, Downloading 
    }
}
