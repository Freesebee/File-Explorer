using Lab1.Dialogs;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TreeMenuService _treeMenuService;
        
        public MainWindow()
        {
            InitializeComponent();
            _treeMenuService = new();
            treeMenu.Items.Add(_treeMenuService.Menu);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                treeMenu.Items.Clear();
                treeMenu.Items.Add(_treeMenuService.CreateTreeMenuItem(dlg.SelectedPath));
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void MenuItem_Click_Read(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (TreeMenuItem)((MenuItem)e.Source).DataContext;

                if (selectedItem.IsDirectory) throw new Exception("You cannot read directiories");
                
                var path = selectedItem.Path;

                itemTextBox.Text = _treeMenuService.ReadFile(path);
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (TreeMenuItem)((MenuItem)e.Source).DataContext;

                _treeMenuService.DeleteItemFromMenu(selectedItem);
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void MenuItem_Click_Create(object sender, RoutedEventArgs e)
        {
            try
            {
                var parentItem = (TreeMenuItem)((MenuItem)e.Source).DataContext;

                if (!parentItem.IsDirectory) throw new Exception("You can only create files within a directory");

                CreateItemFormDialog inputDialog = new(parentItem);

                if (inputDialog.ShowDialog() is null or false) return;

                var newItem = inputDialog.NewItem!;

                if (newItem.IsDirectory)
                {
                    Directory.CreateDirectory(newItem.Path);
                }
                else
                {
                    File.CreateText(newItem.Path);
                }

                parentItem.Items.Add(newItem);
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void treeMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var treeView = (System.Windows.Controls.TreeView)sender;

                if((TreeMenuItem)treeView.SelectedItem is not null)
                {
                    FileAttributesRash.Text = ((TreeMenuItem)treeView.SelectedItem).GetRashString;
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessageDialog(ex);
            }
        }

        private void DisplayErrorMessageDialog(Exception exception)
        {
            string messageBoxText = exception.Message;
            string caption = "Błąd aplikacji";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;

            System.Windows.MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }
    }
}