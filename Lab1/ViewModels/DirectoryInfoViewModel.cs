using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Shapes;

namespace Lab1
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public uint Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;

                    NotifyPropertyChanged(nameof(Count));
                }
            }
        }
        private uint _count;

        public new FileSystemInfo Model
        {
            get => base.Model;
            set
            {
                Size = GetDirectorySize(value);
                Count = (uint)Directory.GetDirectories(value.FullName).Length;
                
                base.Model = value;
            }
        }
        private long GetDirectorySize(FileSystemInfo model)
        {
            return new DirectoryInfo(model.FullName)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Sum(file => file.Length);
        }

        public Exception? Exception { get; private set; }

        public override string ImageSource => "/Resources/Images/DirIcon.jpg";

        public bool Open(string path)
        {
            bool result = false;

            try
            {
                Items.Clear();

                Model = new DirectoryInfo(path);

                foreach (var dirName in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(dirName);

                    var itemViewModel = new DirectoryInfoViewModel
                    {
                        Model = dirInfo,
                    };

                    itemViewModel.Open(dirInfo.FullName);

                    Items.Add(itemViewModel);
                }

                foreach (var fileName in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(fileName);

                    FileInfoViewModel itemViewModel = new FileInfoViewModel();

                    itemViewModel.Model = fileInfo;

                    Items.Add(itemViewModel);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            return result;
        }

        public void Sort(SortOptions sortOptions)
        {
            try
            {
                IOrderedEnumerable<FileSystemInfoViewModel>? query = null;
                if (sortOptions.Direction is SortOrder.Ascending)
                    switch (sortOptions.SortBy)
                    {
                        case SortBy.Alphabetic:
                            query = Items.OrderBy(x => x.Name);
                            break;

                        case SortBy.Size:
                            query = Items.OrderBy(x => x.Size);
                            break;

                        case SortBy.Extension:
                            query = Items.OrderBy(x => x.Extension);
                            break;

                        case SortBy.Date:
                            query = Items.OrderBy(x => x.LastWriteTime);
                            break;

                        default: throw new NotImplementedException();
                    }
                else if (sortOptions.Direction is SortOrder.Descending)
                {
                    switch (sortOptions.SortBy)
                    {
                        case SortBy.Alphabetic:
                            query = Items.OrderByDescending(x => x.Name);
                            break;

                        case SortBy.Size:
                            query = Items.OrderByDescending(x => x.Size);
                            break;

                        case SortBy.Extension:
                            query = Items.OrderByDescending(x => x.Extension);
                            break;

                        case SortBy.Date:
                            query = Items.OrderByDescending(x => x.LastWriteTime);
                            break;

                        default: throw new NotImplementedException();
                    }
                }

                Items = new ObservableCollection<FileSystemInfoViewModel>(query!);

                foreach (var itemViewModel in Items)
                {
                    if (itemViewModel is DirectoryInfoViewModel)
                    {
                        ((DirectoryInfoViewModel)itemViewModel).Sort(sortOptions);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            NotifyPropertyChanged(nameof(Items));
        }

    }
}
