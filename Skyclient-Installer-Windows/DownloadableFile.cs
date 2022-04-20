using System;
using System.IO;

namespace Skyclient
{
    public class DownloadableFile
    {
        public static int CreateGuid(string filename, string downloadlink)
        {
            return (Path.GetFileName(filename) + "|" + downloadlink).GetHashCode();
        }


        public bool CancelDownload = false;
        public string FileDestination;
        public string FileSource;
        public int Guid;

        public DownloadableFile(string filedest, string filesrc)
        {
            FileDestination = filedest;
            FileSource = filesrc;
            this.Guid = CreateGuid(filedest, filesrc);
        }
    }
}
