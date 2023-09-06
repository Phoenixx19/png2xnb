using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace png2xnb.Models
{
    public class IOWrapper
    {
        public static bool IsFile(string path)
        {
            return File.Exists(path) && !IsExistingDirectory(path);
        }

        public static bool IsExistingDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
