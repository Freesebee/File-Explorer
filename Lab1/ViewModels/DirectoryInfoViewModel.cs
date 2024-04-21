using System.Collections.ObjectModel;
using System.IO;

namespace Lab1
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();


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
    }
}
