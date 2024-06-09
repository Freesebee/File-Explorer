using Lab1.Models;
using System.Windows;

namespace Lab1.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for UserFilePermissionsDialog.xaml
    /// </summary>
    public partial class UserFilePermissionsDialog : Window
    {
        public class UserFilePermissionsDialogResult
        {
            public List<UserFilePermissionsViewModel> Permissions { get; set; } = new();
        }

        public UserFilePermissionsDialogResult? Result { get; set; }

        public List<UserFilePermissionsViewModel> PermissionsDataSource = new();

        public UserFilePermissionsDialog(List<UserFilePermissionsViewModel> permissions, FileInfoViewModel fileModel)
        {
            InitializeComponent();

            PermissionsDataSource = permissions;

            userDataGrid.ItemsSource = PermissionsDataSource;
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Result = new() { Permissions = PermissionsDataSource };

            DialogResult = true;
        }
    }
}
