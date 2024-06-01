using Lab1.Commands;
using Lab1.Dialogs;
using Lab1.Resources;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;

namespace Lab1
{
    public class FileExplorer : ViewModelBase
    {
        public DirectoryInfoViewModel? Root { get; set; }
        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value != null)
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        NotifyPropertyChanged();
                    }
            }
        }

        public SortOptions Sorting
        {
            get => Sorting;
            set
            {
                if (value is not null)
                {
                    Sorting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RelayCommand OpenRootFolderCommand { get; private set; }
        public RelayCommand SortRootFolderCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }

        private SortOptions _sorting;
        private FileSystemWatcher? _watcher;
        private Uri? _rootUri;

        public event EventHandler<FileInfoViewModel> OnOpenFileRequest;

        public FileExplorer() : base()
        {
            OpenRootFolderCommand = new(OpenRootFolderExecute);
            SortRootFolderCommand = new(SortRootFolderExecute);
            OpenFileCommand = new(OpenFileExecute);
        }

        public void OpenRoot(string path)
        {
            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            _rootUri = new Uri(path);

            Root = new DirectoryInfoViewModel(this);
            Root.Open(path);

            _watcher.Created += OnFileSystemChanged;
            _watcher.Renamed += OnFileSystemChanged;
            _watcher.Deleted += OnFileSystemChanged;
            _watcher.Changed += OnFileSystemChanged;
            _watcher.Error += Watcher_Error;

            NotifyPropertyChanged(nameof(Root));
        }

        public object GetFileContent(FileInfoViewModel viewModel)
        {
            var extension = viewModel.Extension?.ToLower();
            if (TextFilesExtensions.Contains(extension))
            {
                return GetTextFileContent(viewModel);
            }
            return null;
        }
        
        private string GetTextFileContent(FileInfoViewModel model)
        {
            using (var textReader = File.OpenText(model.Model.FullName))
            {
                return textReader.ReadToEnd();
            }
        }

        public static string OpenFile(FileInfoViewModel model)
        {
            using (var textReader = File.OpenText(model.Model.FullName))
            {
                return textReader.ReadToEnd();
            }
        }

        public static void Delete(FileSystemInfoViewModel vModel)
        {
            if (vModel is FileInfoViewModel)
            {
                File.Delete(vModel.Model.FullName);
            }
            else if (vModel is DirectoryInfoViewModel)
            {
                Directory.Delete(vModel.Model.FullName);
            }
            else throw new NotImplementedException();
        }

        public void Create(DirectoryInfoViewModel parentDir, string name, bool isFile, IEnumerable<FileAttributes> attributes)
        {
            FileSystemInfoViewModel item = isFile
                ? new FileInfoViewModel(this)
                : new DirectoryInfoViewModel(this);

            item.Caption = name;

            var path = parentDir.Model.FullName + $"\\{item.Caption}";

            if (item is DirectoryInfoViewModel)
            {
                Directory.CreateDirectory(path);

                var dirInfo = new DirectoryInfo(path);

                foreach (var attribute in attributes) dirInfo.Attributes |= attribute;
            }
            else if (item is FileInfoViewModel)
            {
                File.CreateText(path);

                foreach (var attribute in attributes) File.SetAttributes(path, attribute);
            }
            else throw new NotImplementedException();
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => OnEventFileSystemChanged(e));
        }

        private void OnEventFileSystemChanged(FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    OnFileSystemChanged(e);
                    break;

                case WatcherChangeTypes.Created:
                    OnFileSystemCreated(e);
                    break;

                case WatcherChangeTypes.Deleted:
                    OnFileSystemDeleted(e);
                    break;

                case WatcherChangeTypes.Renamed:
                    OnFileSystemRenamed(e);
                    break;

                default:
                    throw new NotImplementedException();
            };
        }

        private void OnFileSystemChanged(FileSystemEventArgs e)
        {
            Console.WriteLine(e);
        }

        private void OnFileSystemRenamed(FileSystemEventArgs args)
        {
            var itemWithParent = FindItemAndParent(args);

            //itemWithParent.parent.Items
            //.RemoveAt(itemWithParent.parent.Items.IndexOf(itemWithParent.item));

            //itemWithParent.item.Caption = args.FullPath; //todo test then remove
            var isDir = File
                .GetAttributes(args.FullPath)
                .HasFlag(FileAttributes.Directory);

            itemWithParent.item.Model = isDir
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath);

            //itemWithParent.parent.Items.Add(itemWithParent.item);

            //NotifyPropertyChanged(nameof(itemWithParent.parent.Items)); //todo test 
            NotifyPropertyChanged(nameof(Root)); //todo test
        }

        private void OnFileSystemDeleted(FileSystemEventArgs args)
        {
            var itemWithParent = FindItemAndParent(args);

            itemWithParent.parent.Items
                .RemoveAt(itemWithParent.parent.Items.IndexOf(itemWithParent.item));

            NotifyPropertyChanged(nameof(Root));
        }

        private void OnFileSystemCreated(FileSystemEventArgs args)
        {
            var searchItemPath = new Uri(args.FullPath);

            var relativeSegments = searchItemPath.Segments
                .Where(x => !_rootUri.Segments.Contains(x))
                .Select(Uri.UnescapeDataString)
                .ToList();

            var iterations = uint.MinValue;
            var currentSegIdx = 1;//[0] is root path but with '/' at the end
            var currentDir = Root;
            
            if(relativeSegments.Count > 2)
            {
                while (true)
                {
                    var currentSeg = relativeSegments[currentSegIdx];
                    var item = currentDir.Items.FirstOrDefault(x => x.Caption == currentSeg.Remove(currentSeg.Length - 1));

                    currentDir = item as DirectoryInfoViewModel;

                    if (currentSegIdx == relativeSegments.Count - 2) //parent of new file
                    {
                        break;
                    }
                    else
                    {
                        currentSegIdx++;
                    }

                    if (iterations >= uint.MaxValue) throw new OutOfMemoryException();
                };
            }
            
            var isDir = File
                .GetAttributes(args.FullPath)
                .HasFlag(FileAttributes.Directory);

            FileSystemInfo model = isDir
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath);

            FileSystemInfoViewModel newItem;

            if (isDir)
            {
                newItem = new DirectoryInfoViewModel(this);
                ((DirectoryInfoViewModel)newItem).Open(args.FullPath);
            }
            else
            {
                newItem = new FileInfoViewModel(this);

                var fileInfo = new FileInfo(args.FullPath);

                FileInfoViewModel itemViewModel = new FileInfoViewModel(this);

                newItem.Model = fileInfo;

            }

            currentDir.Items.Add(newItem);

            NotifyPropertyChanged(nameof(Root));
        }

        private (FileSystemInfoViewModel? item, DirectoryInfoViewModel parent) FindItemAndParent(FileSystemEventArgs args)
        {
            var renamedArgs = args as RenamedEventArgs;
            var oldPath = new Uri(renamedArgs is not null ? renamedArgs.OldFullPath : args.FullPath);

            var relativeSegments = oldPath.Segments
                .Where(x => !_rootUri!.Segments.Contains(x))
                .Select(Uri.UnescapeDataString)
                .ToList();

            var iterations = uint.MinValue;
            var currentSegIdx = 1;//coz [0] is root path but with '/' at the end
            var currentDir = Root;

            FileSystemInfoViewModel? result = null;

            while (result is null)
            {
                var currentSeg = relativeSegments[currentSegIdx];

                if (currentSegIdx < relativeSegments.Count - 1) currentSeg = currentSeg.Remove(currentSeg.Length - 1); //info: removes '/' at the end for directories

                var oldItem = currentDir!.Items.First(x => x.Model.Name == currentSeg);

                if (oldItem is null) throw new ArgumentException("Cannot find matching element");

                if (currentSeg == relativeSegments.Last())
                {
                    result = oldItem;
                }
                else
                {
                    currentDir = (DirectoryInfoViewModel)oldItem;
                    currentSegIdx++;
                }

                if (iterations >= uint.MaxValue) throw new OutOfMemoryException();
            };

            return (result, currentDir!);
        }

        private void OpenRootFolderExecute(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory };

            if (dlg.ShowDialog() == DialogResult.Cancel) return;

            OpenRoot(dlg.SelectedPath);
        }

        private void SortRootFolderExecute(object obj)
        {
            var inputDialog = new SortOptionsDialog(_sorting);

            if (inputDialog.ShowDialog() is null or false) return;

            _sorting = inputDialog.SortOptions;

            Root!.Sort(_sorting);

            NotifyPropertyChanged(nameof(Root));
        }


        public static readonly string[] TextFilesExtensions = new string[] { ".txt", ".ini", ".log" };

        private void OpenFileExecute(object obj)
        {
            OnOpenFileRequest.Invoke(this, (FileInfoViewModel)obj);
        }

        private bool OpenFileCanExecute(object parameter)
        {
            if (parameter is FileInfoViewModel viewModel)
            {
                var extension = viewModel.Extension?.ToLower();
                return TextFilesExtensions.Contains(extension);
            }
            return false;
        }

    }
}