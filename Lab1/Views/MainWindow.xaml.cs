using Lab1.BLL;
using Lab1.Dialogs;
using Lab1.Resources;
using Lab1.Views.Dialogs;
using Microsoft.VisualBasic.ApplicationServices;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Lab1.DAL;
using System.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Lab1.Extensions;

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileExplorer _fileExplorer;
        private readonly FileManager _fileManager;
        private readonly AppDbContext _context;

        public MainWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();
            _context.Database.EnsureCreated();

            _fileExplorer = new();

            DataContext = _fileExplorer;

            _fileExplorer.PropertyChanged += _fileExplorer_PropertyChanged;
            _fileExplorer.OnOpenFileRequest += _fileExplorer_OnOpenFileRequest;
            _fileExplorer.OnFileChange += _fileExplorer_OnFileChange;
            _fileExplorer.OnRegisterUser += _fileManager_OnRegisterUser;
            _fileExplorer.OnListUsers += _fileManager_OnListUsers;
            _fileExplorer.OnModifyMetadataCommand += _fileManager_OnModifyMetadata;

            _fileManager = InitFileManager();
        }


        private FileManager InitFileManager()
        {
            var fileManager = new FileManager(_context);

            var dialog = new SignInDialog(fileManager);

            var result = dialog.ShowDialog();

            if (result is null)
            {
                return fileManager;
            }
            else if (result is false)
            {
                return fileManager;
            }

            _fileExplorer.StatusMessage = dialog.Result.Message;

            return fileManager;
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory };

                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

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

                //itemTextBox.Text = FileExplorer.OpenFile(fileInfo); //todo remove whole methods
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

                _fileExplorer.Create(
                    parentDir,
                    inputDialog.Name,
                    inputDialog.IsFileSelected,
                    inputDialog.ModelFileAttributes);
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

        private void _fileExplorer_OnOpenFileRequest(object sender, FileInfoViewModel viewModel)
        {
            var content = _fileExplorer.GetFileContent(viewModel);
            if (content is string text)
            {
                var textView = new TextBlock { Text = text };
                ContentViewer.Content = textView;
            }
        }

        private void _fileExplorer_OnFileChange(object? sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException(); //todo
        }

        private void _fileManager_OnRegisterUser(object? sender, EventArgs e)
        {
            var dialog = new RegisterUserDialog(_fileManager);

            var result = dialog.ShowDialog();

            if (result is null)
            {
                return;
            }
            else if (result is false)
            {
                return;
            }

            _fileManager.RegisterUser(new Models.UserRegistraionModel()
            {
                IPAddress = dialog.Result.IPAddress,
                Login = dialog.Result.Login,
                PasswordHash = PasswordExtensions.MD5Hash(dialog.Result.Password),
                IsHost = false
            });

            _fileExplorer.StatusMessage = string.Format(Strings.User_created, dialog.Result.Login);
        }

        private void _fileManager_OnListUsers(object? sender, EventArgs e)
        {
            var users = _context.Users.ToList();

            var dialog = new UserListDialog(users);

            var result = dialog.ShowDialog();

            if (result is null or false) return;

            foreach (var user in dialog.Result.Users)
            {
                var entity = _context.Users.First(x => x.Id == user.Id);
                entity.Login = user.Login;
                entity.IPAddress = user.IPAdress;
                entity.IsActive = !user.IsBlocked;
            }

            _context.SaveChanges();
        }

        private void _fileManager_OnModifyMetadata(object? sender, FileInfoViewModel e)
        {
            var metadata = _context.FileMetadata.ToList();

            var dialog = new MetadataDialog(metadata);

            var result = dialog.ShowDialog();

            if (result is null or false) return;

            var entity =_context.FileMetadata.FirstOrDefault(x => x.Id == dialog.Result!.Metadata.Id);
            if (entity is null)
            {
                _context.FileMetadata.Add(dialog.Result!.Metadata);
            }

            _context.SaveChanges();
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            _context.Dispose();

            base.OnClosing(e);
        }
    }
}