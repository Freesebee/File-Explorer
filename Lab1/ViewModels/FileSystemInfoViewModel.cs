﻿using System.IO;
using System.Windows.Shapes;

namespace Lab1
{
    public class FileSystemInfoViewModel : ViewModelBase
    {
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;

                    NotifyPropertyChanged(nameof(Caption));
                }
            }
        }
        private string _caption;

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set
            {
                if (_lastWriteTime != value)
                {
                    _lastWriteTime = value;

                    NotifyPropertyChanged(nameof(LastWriteTime));
                }
            }
        }
        private DateTime _lastWriteTime;

        public virtual string ImageSource { get; } = string.Empty;

        public FileSystemInfo Model
        {
            get { return _fileSystemInfo; }
            set
            {
                if (_fileSystemInfo != value)
                {
                    _fileSystemInfo = value;
                    LastWriteTime = value.LastWriteTime;
                    Caption = value.Name;

                    NotifyPropertyChanged(nameof(Model));
                }
            }
        }
        private FileSystemInfo _fileSystemInfo;

        public string GetRashString
        {
            get
            {
                var Attributes = File.GetAttributes(Model.FullName);

                var text = string.Empty;

                text += Attributes == FileAttributes.ReadOnly ? "r" : "-";
                text += Attributes == FileAttributes.Archive ? "a" : "-";
                text += Attributes == FileAttributes.Hidden ? "h" : "-";
                text += Attributes == FileAttributes.System ? "s" : "-";

                return text;
            }
        }
    }
}