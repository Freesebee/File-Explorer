using System.IO;
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
        private string _caption = string.Empty;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;

                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }
        private string _name = string.Empty;

        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    _extension = value;

                    NotifyPropertyChanged(nameof(Extension));
                }
            }
        }
        private string _extension = string.Empty;

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

        public long Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;

                    NotifyPropertyChanged(nameof(Size));
                }
            }
        }
        private long _size;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;

                    NotifyPropertyChanged(nameof(StatusMessage));
                }
            }
        }
        private string _statusMessage;

        public ViewModelBase Owner { get; private set; }

        public FileExplorer? OwnerExplorer
        {
            get
            {
                var owner = Owner;

                while (owner is DirectoryInfoViewModel ownerDirectory)
                {
                    if (ownerDirectory.Owner is FileExplorer explorer)
                        return explorer;

                    owner = ownerDirectory.Owner;
                }

                return null;
            }
        }

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
                    Name = value.Name;
                    Extension = value.Extension;

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

        public FileSystemInfoViewModel(ViewModelBase owner)
        {
            Owner = owner;
        }

    }
}
