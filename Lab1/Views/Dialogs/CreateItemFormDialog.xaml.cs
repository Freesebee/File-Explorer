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
    public IEnumerable<FileAttributes> ModelFileAttributes => _modelFileAttributes;
    private List<FileAttributes> _modelFileAttributes = new();

    private readonly FileSystemInfoViewModel _parent;
    public bool RadioButton1IsChecked { get; set; }
    public bool RadioButton2IsChecked { get; set; }

    public bool IsFileSelected {  get; set; }
    public string Name { get; set; }

    public CreateItemFormDialog(FileSystemInfoViewModel parent)
    {
        InitializeComponent();
        _parent = parent;
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Button_Click_Ok(object sender, RoutedEventArgs e)
    {
        if ((bool)isReadonly.IsChecked!) _modelFileAttributes.Add(FileAttributes.ReadOnly);
        if ((bool)isArchive.IsChecked!) _modelFileAttributes.Add(FileAttributes.Archive);
        if ((bool)isHidden.IsChecked!) _modelFileAttributes.Add(FileAttributes.Hidden);
        if ((bool)isSystem.IsChecked!) _modelFileAttributes.Add(FileAttributes.System);

        IsFileSelected = !(bool)IsDirectory.IsChecked!;

        Name = nameTextBox.Text;

        DialogResult = true;
    }
}
