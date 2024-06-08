using Lab1.Commands;
using Lab1.Dialogs;
using Lab1.Resources;
using Lab1.Views.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;

namespace Lab1
{
    public class FileExplorer : ViewModelBase
    {
        public DirectoryInfoViewModel? Root { get; set; }

        private CancellationTokenSource _cancellationTokenSrc;
        private CancellationToken CancellationToken => _cancellationTokenSrc.Token;

        private bool _isRunningTaskButtonEnabled;

        public bool IsRunningTaskButtonEnabled
        {
            get { return _isRunningTaskButtonEnabled; }
            set
            {
                _isRunningTaskButtonEnabled = value;
                NotifyPropertyChanged(nameof(IsRunningTaskButtonEnabled));
            }
        }

        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value != null)
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        NotifyPropertyChanged(nameof(Lang));
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
        public RelayCommand CancelTaskCommand { get; private set; }
        public RelayCommand RegisterUserCommand { get; private set; }
        public RelayCommand ListUserCommand { get; private set; }

        public event EventHandler<FileInfoViewModel> OnOpenFileRequest;
        public event EventHandler<FileSystemEventArgs> OnFileChange;
        public event EventHandler OnRegisterUser;
        public event EventHandler OnListUsers;

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

        private string _statusMessage = Strings.Ready;

        private SortOptions _sorting;
        private FileSystemWatcher? _watcher;
        private Uri? _rootUri;


        public FileExplorer() : base()
        {
            _isRunningTaskButtonEnabled = false;
            _cancellationTokenSrc = new CancellationTokenSource();
            OpenRootFolderCommand = new(OpenRootFolderExecuteAsync);
            SortRootFolderCommand = new(SortRootFolderExecuteAsync, SortRootFolderCanExecute);
            OpenFileCommand = new(OpenFileExecute, OpenFileCanExecute);
            CancelTaskCommand = new(CancelTaskExecute);
            RegisterUserCommand = new(RegisterUserExecute, RegisterUserCanExecute);
            ListUserCommand = new(ListUserExecute, ListUserCanExecute);
        }

        public void OpenRoot(string path)
        {
            StatusMessage = $"{Strings.Loading} {path}";

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

            Root.PropertyChanged += Root_PropertyChanged;

            StatusMessage = Strings.Ready;

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
            OnFileChange.Invoke(this, e);
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

            var isDir = File
                .GetAttributes(args.FullPath)
                .HasFlag(FileAttributes.Directory);

            itemWithParent.item.Model = isDir
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath);

            var renameArgs = (RenamedEventArgs)args;

            StatusMessage = string.Format(Strings.File_renamed, renameArgs.OldName, renameArgs.Name);

            NotifyPropertyChanged(nameof(Root)); //todo test
        }

        private void OnFileSystemDeleted(FileSystemEventArgs args)
        {
            var itemWithParent = FindItemAndParent(args);

            itemWithParent.parent.Items
                .RemoveAt(itemWithParent.parent.Items.IndexOf(itemWithParent.item));

            StatusMessage = $"{Strings.File_deleted} {args.FullPath}";

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

            if (relativeSegments.Count > 2)
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

            StatusMessage = Strings.File_created +  " " + args.FullPath;

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

            FileSystemInfoViewModel? targetItem = null;

            while (targetItem is null)
            {
                var currentSeg = relativeSegments[currentSegIdx];

                if (currentSegIdx < relativeSegments.Count - 1) currentSeg = currentSeg.Remove(currentSeg.Length - 1); //info: removes '/' at the end for directories

                var nextItem = currentDir!.Items.First(x => x.Model.Name == currentSeg);

                if (nextItem is null) throw new ArgumentException("Cannot find matching element");

                if (currentSeg == relativeSegments.Last())
                {
                    targetItem = nextItem;
                }
                else
                {
                    currentDir = (DirectoryInfoViewModel)nextItem;
                    currentSegIdx++;
                }

                if (iterations >= uint.MaxValue) throw new OutOfMemoryException();
            };

            return (targetItem, currentDir!);
        }

        private async void OpenRootFolderExecuteAsync(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            await Task.Factory.StartNew(() => OpenRoot(dlg.SelectedPath));
        }

        private async void SortRootFolderExecuteAsync(object? obj)
        {
            _cancellationTokenSrc = new();

            StatusMessage = Strings.Sorting_directory;

            var inputDialog = new SortOptionsDialog(_sorting);

            if (inputDialog.ShowDialog() is null or false) return;

            _sorting = inputDialog.SortOptions;

            IsRunningTaskButtonEnabled = true;

            try
            {
                await Task.Factory.StartNew(() => Root!.Sort(_sorting, CancellationToken), CancellationToken);
                StatusMessage = $"{Strings.Directory_sorted}";
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
                StatusMessage = $"{Strings.Task_cancelled}";
            }
            catch (AggregateException)
            {
                Debug.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
                StatusMessage = $"{Strings.Task_cancelled}";
            }
            finally
            {
                IsRunningTaskButtonEnabled = false;
                _cancellationTokenSrc.Dispose();
            }

            NotifyPropertyChanged(nameof(Root));
        }

        private bool SortRootFolderCanExecute(object? obj) 
        {
            return Root != null && Root.Items != null && Root.Items.Count > 0;
        }

        private void CancelTaskExecute(object obj)
        {
            _cancellationTokenSrc.Cancel();
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

        private void Root_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(StatusMessage) && sender is FileSystemInfoViewModel viewModel)
            {
                StatusMessage = viewModel.StatusMessage;
            }
        }

        private bool IsCurrentUserHost()
        {
            return true;
        }

        private void RegisterUserExecute(object obj)
        {
            OnRegisterUser.Invoke(this, null);
        }

        private bool RegisterUserCanExecute(object parameter)
        {
            return IsCurrentUserHost();
        }

        private void ListUserExecute(object obj)
        {
            OnListUsers.Invoke(this, null);
        }

        private bool ListUserCanExecute(object obj)
        {
            return IsCurrentUserHost();
        }

    }
}