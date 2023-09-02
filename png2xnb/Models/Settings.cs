using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace png2xnb.Models
{
    public class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            if (Instance != null)
                return;

            // read xml
            // ...

            Instance = this;
        }

        [XmlIgnore]
        public static Settings Instance { get; set; }


        #region properties
        /// <summary>
        /// Toggle for auto output
        /// </summary>
        public bool AutoOutput
        {
            get => Properties.Settings.Default.AutoOutput;
            set
            {
                Properties.Settings.Default.AutoOutput = value;
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Toggle for auto convert
        /// </summary>
        public bool AutoConvert
        {
            get => Properties.Settings.Default.AutoConvert;
            set
            {
                Properties.Settings.Default.AutoConvert = value;
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Auto convert directory
        /// </summary>
        public string DefaultDir
        {
            get => Properties.Settings.Default.DefaultDir;
            set
            {
                Properties.Settings.Default.DefaultDir = value;
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }

        public bool IsCompressed
        {
            get => Properties.Settings.Default.IsCompressed;
            set
            {
                Properties.Settings.Default.IsCompressed = value;
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Format type for converting to xnb, read <see href="https://shawnhargreaves.com/blog/reach-vs-hidef.html">Reach vs HiDef</see>
        /// </summary>
        public ProfileType Format
        {
            get
            {
                if (!Enum.TryParse(Properties.Settings.Default.Format, true, out ProfileType type))
                    throw new ArgumentException("Could not get the ProfileType requested from Format!\nSet the Format to Reach in the config file.");
                return type;
            }
            set
            {
                Properties.Settings.Default.Format = value.ToString();
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Toggle for premultiplying alpha
        /// </summary>
        public bool PremultiplyAlpha
        {
            get => Properties.Settings.Default.PremultiplyAlpha;
            set
            {
                Properties.Settings.Default.PremultiplyAlpha = value;
                OnPropertyChanged();
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
