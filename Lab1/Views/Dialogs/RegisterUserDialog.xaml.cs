using Lab1.BLL;
using Lab1.Extensions;
using Lab1.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab1.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for RegisterUserDialog.xaml
    /// </summary>
    public partial class RegisterUserDialog : Window
    {
        public RegisterUserDialog(FileManager fileManager)
        {
            InitializeComponent();
            _fileManager = fileManager;
        }

        public class RegisterDialogResult
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string IPAddress { get; set; }
            public string Message { get; set; }
        }

        public RegisterDialogResult? Result;
        private readonly FileManager _fileManager;

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Result = new();

            Result.Login = loginTextBox.Text;
            Result.Password = passwordBox.Password;
            Result.IPAddress = ipTextBox.Text;

            DialogResult = true;
        }
    }
}
