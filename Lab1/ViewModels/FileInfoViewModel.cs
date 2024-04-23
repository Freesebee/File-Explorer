using System.IO;

namespace Lab1
{
    public class FileInfoViewModel : FileSystemInfoViewModel
    {
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
    }
}
