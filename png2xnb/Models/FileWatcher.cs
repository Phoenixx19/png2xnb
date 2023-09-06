using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IOPath = System.IO.Path;

namespace png2xnb.Models
{
    public class FileWatcher : FileSystemWatcher
    {
        public FileWatcher() : base()
        {
            NotifyFilter =
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName;


            Changed += new FileSystemEventHandler(WaitForChanged);
            Created += new FileSystemEventHandler(WaitForChanged);
            Deleted += new FileSystemEventHandler(WaitForChanged);
            Renamed += new RenamedEventHandler(WaitForChanged);
        }

        public void Update(string path)
        {
            EnableRaisingEvents = false;

            if (IOWrapper.IsFile(path))
            {
                Path = IOPath.GetDirectoryName(path);
                Filter = IOPath.GetFileName(path);
                IncludeSubdirectories = false;
            }
            else
            {
                Path = path;
                Filter = "*.*";
                IncludeSubdirectories = true;
            }

            EnableRaisingEvents = true;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/10982104/wait-until-file-is-completely-written
        /// </summary>
        private void WaitForChanged(object sender, FileSystemEventArgs e)
        {
            if (IOWrapper.IsExistingDirectory(e.FullPath))
                return;

            var fileInfo = new FileInfo(e.FullPath);

            while (IOWrapper.IsFileLocked(fileInfo))
            {
                Thread.Sleep(1000);
            }
        }
    }
}
