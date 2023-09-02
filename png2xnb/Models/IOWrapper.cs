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
    }
}
