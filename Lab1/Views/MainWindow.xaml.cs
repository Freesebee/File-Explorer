using Lab1.Dialogs;
using Lab1.Resources;
using Microsoft.VisualBasic.ApplicationServices;
using System.ComponentModel;
using System.Globalization;
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
        private readonly FileExplorer _fileExplorer;

        public MainWindow()
        {
            InitializeComponent();

            _fileExplorer = new();

            DataContext = _fileExplorer;

            _fileExplorer.PropertyChanged += _fileExplorer_PropertyChanged;
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory };

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                _fileExplorer.OpenRoot(dlg.SelectedPath);
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
                var fileInfo = (FileInfoViewModel)((MenuItem)e.Source).DataContext;

                itemTextBox.Text = FileExplorer.OpenFile(fileInfo);
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
                var fileInfo = ((MenuItem)e.Source).DataContext as FileSystemInfoViewModel;

                if (fileInfo is not null)
                {
                    FileExplorer.Delete(fileInfo);
                }
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
                var parentDir = (DirectoryInfoViewModel)((MenuItem)e.Source).DataContext;

                var inputDialog = new CreateItemFormDialog(parentDir);

                if (inputDialog.ShowDialog() is null or false) return;

                FileExplorer.Create(parentDir, inputDialog.NewModel, inputDialog.ModelFileAttributes);
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
                var fileInfo = e.NewValue as FileSystemInfoViewModel;
                if (fileInfo is not null)
                {
                    FileAttributesRash.Text = fileInfo.GetRashString;
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

        private void _fileExplorer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FileExplorer.Lang):
                    CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
                    break;

                case nameof(FileExplorer.Sorting):
                    _fileExplorer.Sorting = new();
                    break;

                default: break;
            }
        }
    }
}