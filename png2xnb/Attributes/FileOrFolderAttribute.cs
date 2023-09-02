using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace png2xnb.Attributes
{
    public class FileOrFolderAttribute : ValidationAttribute
    {
        public FileOrFolderAttribute() { }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            return Directory.Exists(value.ToString()) || File.Exists(value.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return "The following value has to be an existing file or folder path";
        }
    }
}