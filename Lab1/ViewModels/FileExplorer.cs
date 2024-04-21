using Lab1.Commands;
using Lab1.Resources;
using System.Globalization;
using System.IO;

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
        
        public RelayCommand OpenRootFolderCommand { get; private set; }
        public RelayCommand SortRootFolderCommand { get; private set; }

        private FileSystemWatcher? _watcher;
        private Uri? _rootUri;

        public FileExplorer() : base()
        {
            OpenRootFolderCommand = new(OpenRootFolderExecute);
            SortRootFolderCommand = new(SortRootFolderExecute);
        }

        public void OpenRoot(string path)
        {
            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            _rootUri = new Uri(path);

            Root = new DirectoryInfoViewModel();
            Root.Open(path);

            _watcher.Created += OnFileSystemChanged;
            _watcher.Renamed += OnFileSystemChanged;
            _watcher.Deleted += OnFileSystemChanged;
            _watcher.Changed += OnFileSystemChanged;
            _watcher.Error += Watcher_Error;

            NotifyPropertyChanged(nameof(Root));
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

        public static void Create(DirectoryInfoViewModel parentDir, FileSystemInfoViewModel item, IEnumerable<FileAttributes> attributes)
        {
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

            var relativeSegments = searchItemPath.Segments.Where(x => !_rootUri.Segments.Contains(x)).ToList();

            var iterations = uint.MinValue;
            var currentSegIdx = 1;//[0] is root path but with '/' at the end
            var currentDir = Root;
            while (true)
            {
                var currentSeg = relativeSegments[currentSegIdx]
                    .Replace("/", "") //dir have '/' at the end
                    .Replace("%20", " "); //lazy encoding fix

                var item = currentDir.Items.FirstOrDefault(x => x.Caption == currentSeg);

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

            var isDir = File
                .GetAttributes(args.FullPath)
                .HasFlag(FileAttributes.Directory);

            FileSystemInfo model = isDir
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath);

            FileSystemInfoViewModel newItem;

            if (isDir)
            {
                newItem = new DirectoryInfoViewModel();
                ((DirectoryInfoViewModel)newItem).Open(args.FullPath);
            }
            else
            {
                newItem = new FileInfoViewModel();

                var fileInfo = new FileInfo(args.FullPath);

                FileInfoViewModel itemViewModel = new FileInfoViewModel();

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
                .Select(x => x.Replace("/", "").Replace("%20", " "))
                .ToList();

            var iterations = uint.MinValue;
            var currentSegIdx = 1;//coz [0] is root path but with '/' at the end
            var currentDir = Root;

            FileSystemInfoViewModel? result = null;

            while (result is null)
            {
                var currentSeg = relativeSegments[currentSegIdx];
                var oldItem = currentDir!.Items.First(x => x.Model.Name == currentSeg);

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
            throw new NotImplementedException();
        }
    }
}