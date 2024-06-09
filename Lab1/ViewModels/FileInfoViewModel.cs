using Lab1.Commands;
using System.Data.Common;
using System.IO;
using System.Reflection.Metadata;

namespace Lab1
{
    public class FileInfoViewModel : FileSystemInfoViewModel
    {
        public FileInfoViewModel(ViewModelBase owner) : base(owner)
        {
            OpenFileCommand = new(OpenFileExecute, OpenFileCanExecute);
            ModifyMetadataCommand = new(ModifyMetadataExecute);
        }

        public override string ImageSource => "/Resources/Images/FileIcon.jpg";

        public new FileSystemInfo Model
        {
            get => base.Model;
            set
            {
                Size = new FileInfo(value.FullName).Length;
                base.Model = value;
            }
        }

        public RelayCommand OpenFileCommand { get; private set; }
        public RelayCommand ModifyMetadataCommand { get; private set; }

        private bool OpenFileCanExecute(object parameter)
        {
            return OwnerExplorer.OpenFileCommand.CanExecute(parameter);
        }
        private void OpenFileExecute(object parameter)  
        {
            OwnerExplorer.OpenFileCommand.Execute(parameter);
        }

        private void ModifyMetadataExecute(object obj)
        {
            OwnerExplorer.ModifyMetadataCommand.Execute((FileInfoViewModel)obj);
        }
    }
}
