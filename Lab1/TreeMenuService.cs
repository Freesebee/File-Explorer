using System.IO;
using Path = System.IO.Path;

namespace Lab1
{
    internal sealed class TreeMenuService
    {
        private TreeMenuItem? _menu;
        public TreeMenuItem? Menu => _menu;

        public TreeMenuService(string? path = null)
        {
            if (path is not null)
            {
                _menu = CreateTreeMenuItem(path);
            }
        }

        internal TreeMenuItem CreateTreeMenuItem(string path)
        {

            FileAttributes attr = File.GetAttributes(path);

            TreeMenuItem root = new()
            {
                Header = Path.GetFileName(path),
                Path = path,
                IsDirectory = attr.HasFlag(FileAttributes.Directory),
                Attributes = [attr],
            };

            if (root.IsDirectory)
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(path);

                List<TreeMenuItem> treeMenuItems = new List<TreeMenuItem>();

                foreach (var subDirectory in rootDirectory.GetDirectories())
                {
                    treeMenuItems.Add(CreateTreeMenuItem(subDirectory.FullName));
                }

                foreach (var file in rootDirectory.GetFiles())
                {
                    treeMenuItems.Add(CreateTreeMenuItem(file.FullName));
                }

                treeMenuItems.ForEach(root.Items.Add);
            }

            _menu = root;

            return root;
        }

        internal void DeleteItemFromMenu(TreeMenuItem item)
        {
            FindAndDeleteItem(item, _menu!);
        }

        private void FindAndDeleteItem(TreeMenuItem itemToDelete, TreeMenuItem parent)
        {
            if (parent.Items.Contains(itemToDelete))
            {
                if (itemToDelete.IsDirectory)
                {
                    Directory.Delete(itemToDelete.Path);
                }
                else
                {
                    File.Delete(itemToDelete.Path);
                }

                parent.Items.Remove(itemToDelete);

                return;
            }

            foreach (var item in parent.Items)
            {
                FindAndDeleteItem(itemToDelete, item);
            }
        }
    }
}
