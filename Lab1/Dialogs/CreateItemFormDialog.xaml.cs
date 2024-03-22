using Accessibility;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Lab1.Dialogs;

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
public partial class CreateItemFormDialog : Window
{
    private TreeMenuItem? _newItem;
    public TreeMenuItem? NewItem => _newItem;
    private readonly TreeMenuItem _root;

    public bool RadioButton1IsChecked { get; set; }
    public bool RadioButton2IsChecked { get; set; }

    public CreateItemFormDialog(TreeMenuItem root)
    {
        InitializeComponent();
        _root = root;
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        this.Close();
    }

    private void Button_Click_Ok(object sender, RoutedEventArgs e)
    {
        var itemName = nameTextBox.Text;
        var rootPath = new Uri(_root.Path + "\\"); //idc
        var itemPath = new Uri(rootPath, itemName);

        List<FileAttributes> fileAttributes = [];

        if ((bool)isReadonly.IsChecked!) fileAttributes.Add(FileAttributes.ReadOnly);
        if ((bool)isArchive.IsChecked!) fileAttributes.Add(FileAttributes.Archive);
        if ((bool)isHidden.IsChecked!) fileAttributes.Add(FileAttributes.Hidden);
        if ((bool)isSystem.IsChecked!) fileAttributes.Add(FileAttributes.System);

        var newItem = new TreeMenuItem()
        {
            Header = itemName,
            Path = itemPath.AbsolutePath,
            IsDirectory = (bool)IsDirectory.IsChecked!,
            Attributes = fileAttributes.ToArray(),
        };

        _newItem = newItem;

        DialogResult = true;
    }
}
