using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.Notifications;
using Ookii.Dialogs.Wpf;
using png2xnb.Attributes;
using png2xnb.Models;
using png2xnb.Views;

namespace png2xnb.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public MainWindowViewModel()
        {
            OnErrorsChanged(nameof(Input));
            OnErrorsChanged(nameof(Output));
        }

        #region properties
        private string input;
        private OutputType type;
        private string output;
        private string logMessage = "Ready";
        private ICommand chooseInputFileCommand;
        private ICommand chooseInputFolderCommand;
        private ICommand chooseOutputFileCommand;
        private ICommand chooseOutputFolderCommand;
        private ICommand convertCommand;

        /// <summary>
        /// Input file full path
        /// </summary>

        [Required]
        [FileOrFolder]
        public string Input
        { 
            get => input;
            set
            {
                input = value;
                OnPropertyChanged();
                OnErrorsChanged();
            }
        }

        /// <summary>
        /// Input file type (file or folder)
        /// </summary>
        public OutputType Type
        { 
            get => type; 
            private set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        [Required]
        /// <summary>
        /// Output file full path
        /// </summary>
        public string Output
        { 
            get => output; 
            set
            {
                output = value;
                OnPropertyChanged();
                OnErrorsChanged();
            }
        }

        /// <summary>
        /// Log message string below the convert button
        /// </summary>
        public string LogMessage
        { 
            get => logMessage; 
            set
            {
                logMessage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region commands
        /// <summary>
        /// <see href="https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation"/>
        /// </summary>

        #region input file
        public ICommand ChooseInputFileCommand
        { 
            get
            {
                if (chooseInputFileCommand == null)
                {
                    chooseInputFileCommand = new Models.RelayCommand(
                        p => true,
                        p => this.ChooseInputFile()
                    );
                }
                return chooseInputFileCommand;
            }
        }

        void ChooseInputFile()
        { 
            VistaOpenFileDialog fileDialog = new();

            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "PNG (*.png)|*.png";

            if (fileDialog.ShowDialog() == true)
            {
                Input = fileDialog.FileName;
                Type = OutputType.File;

                if (Settings.Instance.AutoOutput)
                {
                    Output = string.Concat(
                        Path.GetDirectoryName(fileDialog.FileName),
                        "\\",
                        Path.GetFileNameWithoutExtension(fileDialog.FileName),
                        ".xnb"
                    );
                }
            }
        }
        #endregion

        #region input folder
        public ICommand ChooseInputFolderCommand
        {
            get
            {
                if (chooseInputFolderCommand == null)
                {
                    chooseInputFolderCommand = new Models.RelayCommand(
                        p => true,
                        p => this.ChooseInputFolder()
                    );
                }
                return chooseInputFolderCommand;
            }
        }
        void ChooseInputFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog1 = new();

            folderBrowserDialog1.Description = "Select the folder with the PNG files you want to convert.";
            folderBrowserDialog1.SelectedPath = Path.GetFullPath(".");
            folderBrowserDialog1.ShowNewFolderButton = false;

            if (folderBrowserDialog1.ShowDialog() == true)
            {
                Input = folderBrowserDialog1.SelectedPath;
                Type = OutputType.Folder;

                if (Settings.Instance.AutoOutput)
                {
                    Output = string.Concat(
                        Path.GetFullPath(folderBrowserDialog1.SelectedPath),
                        "\\",
                        (Settings.Instance.DefaultDir != null) ? Settings.Instance.DefaultDir + "\\" : string.Empty
                    );
                }
            }
        }
        #endregion

        #region output file
        public ICommand ChooseOutputFileCommand
        {
            get
            {
                if (chooseOutputFileCommand == null)
                {
                    chooseOutputFileCommand = new Models.RelayCommand(
                        p => true,
                        p => this.ChooseOutputFile()
                    );
                }
                return chooseOutputFileCommand;
            }
        }
        void ChooseOutputFile()
        {
            VistaSaveFileDialog openFileDialog1 = new();

            openFileDialog1.Filter = "XNB files (*.xnb)|*.xnb|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                Output = openFileDialog1.FileName;
            }
        }
        #endregion

        #region output folder
        public ICommand ChooseOutputFolderCommand
        {
            get
            {
                if (chooseOutputFolderCommand == null)
                {
                    chooseOutputFolderCommand = new Models.RelayCommand(
                        p => true,
                        p => this.ChooseOutputFolder()
                    );
                }
                return chooseOutputFolderCommand;
            }
        }
        void ChooseOutputFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog1 = new();

            folderBrowserDialog1.Description = "Select the folder where you want to save the XNB files.";
            folderBrowserDialog1.SelectedPath = Path.GetFullPath(".");
            folderBrowserDialog1.ShowNewFolderButton = true;

            if (folderBrowserDialog1.ShowDialog() == true)
            {
                Output = folderBrowserDialog1.SelectedPath;
            }
        }
        #endregion

        #region convert action
        public ICommand ConvertCommand
        {
            get
            {
                if (convertCommand == null)
                {
                    convertCommand = new Models.RelayCommand(
                        p => true,
                        p => this.Convert()
                    );
                }
                return convertCommand;
            }
        }
        void Convert()
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                Log("Please select an input file or folder");
                return;
            }
            else if (string.IsNullOrWhiteSpace(Output))
            {
                Log("Please select an output file or folder");
                return;
            }
            else if (!IOWrapper.IsFile(Input) && !IOWrapper.IsExistingDirectory(Input))
            {
                Log("Input file or folder does not exists: " + Input);
                return;
            }
            
            RunConvert(Input, Output, Settings.Instance.IsCompressed, Settings.Instance.Format, Settings.Instance.PremultiplyAlpha);
        }
        #endregion

        #endregion

        #region png2xnb methods
        private int PngFileConvert(string pngFile, string xnbDirectory, bool compressed, bool reach, bool premultiply)
        {
            string fileName = Path.GetFileNameWithoutExtension(pngFile);
            string xnbFile = Path.Combine(xnbDirectory, fileName + ".xnb");
            return XNBManager.Convert(pngFile, xnbFile, compressed, reach, premultiply);
        }

        private int PngFolderConvert(string pngDirectory, string xnbDirectory, bool compressed, bool reach, bool premultiply)
        {
            string[] files = Directory.GetFiles(pngDirectory, "*.png", SearchOption.AllDirectories);
            int count = 0;
            foreach (string pngFile in files)
            {
                var x = Path.GetRelativePath(pngDirectory, pngFile);

                count += PngFileConvert(pngFile, xnbDirectory, compressed, reach, premultiply);
            }
            return count;
        }

        private void RunConvert(string pngFileOrDir, string xnbFileOrDir, bool compressed, ProfileType type, bool premultiply)
        {
            if (!File.Exists(pngFileOrDir) && !Directory.Exists(pngFileOrDir))
            {
                throw new ArgumentException("The png_file does not exist: " + pngFileOrDir);
            }
            int count;
            bool isReach = type == ProfileType.Reach;
            
            bool isFile = false;
            
            if (IOWrapper.IsFile(pngFileOrDir))
            {
                isFile = true;
                if (IOWrapper.IsExistingDirectory(xnbFileOrDir))
                {
                    count = PngFileConvert(pngFileOrDir, xnbFileOrDir, compressed, isReach, premultiply);
                }
                else
                {
                    count = XNBManager.Convert(pngFileOrDir, xnbFileOrDir, compressed, isReach, premultiply);
                }
            }
            else
            {
                if (!IOWrapper.IsExistingDirectory(xnbFileOrDir))
                    Directory.CreateDirectory(xnbFileOrDir);

                count = PngFolderConvert(pngFileOrDir, xnbFileOrDir, compressed, isReach, premultiply);
            }

            string text = $"Successfully converted to {Path.GetFileName(Output)}.";
            if (!isFile)
            {
                text = "Converted " + count + " file" + (count != 1 ? "s" : "") + Path.GetFullPath(Output);
            }

            if (Settings.Instance.AutoConvert)
            {
                new ToastContentBuilder().AddText(text).Show();
            }
            Log(text);
        }

        public void Log(string message)
        {
            LogMessage = message;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region INotifyDataErrorInfo
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public bool HasErrors
        {
            get => errors.Count > 0;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            var results = new List<ValidationResult>();
            ValidationContext context = new(this) { MemberName = propertyName };

            object value = GetType().GetProperty(propertyName).GetValue(this, null);

            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
                errors[propertyName] = results.Select(x => x.ErrorMessage).ToList();
            else
                errors.Remove(propertyName);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
        }
        #endregion
    }
}
