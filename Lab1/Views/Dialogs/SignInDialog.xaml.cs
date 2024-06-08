using Lab1.BLL;
using Lab1.Extensions;
using Lab1.Resources;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Lab1.Views.Dialogs;

/// <summary>
/// Interaction logic for SignInDialog.xaml
/// </summary>
public partial class SignInDialog : Window
{

    public class SignInDialogResult
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
    }

    public SignInDialogResult? Result;
    private readonly FileManager _fileManager;

    public SignInDialog(FileManager fileManager)
    {
        InitializeComponent();
        _fileManager = fileManager;
    }

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

        var userExists = _fileManager.LoginExists(Result.Login);

        if (userExists)
        {
            bool isSignedIn = _fileManager.SignIn(Result.Login, PasswordExtensions.MD5Hash(Result.Password));

            Result.Message = string.Format(Strings.User_signed_in, Result.Login);
            DialogResult = isSignedIn;
        }
        else
        {
            var isLocalUser = _fileManager.IsLocalUser(Result.Login);
            
            if (isLocalUser)
            {
                var securePassword = new NetworkCredential(Result.Login, Result.Password).SecurePassword;

                if (FileManager.ValidateUsernameAndPassword(Result.Login, securePassword))
                {
                    _fileManager.RegisterUser(new Models.UserRegistraionModel()
                    {
                        PasswordHash = PasswordExtensions.MD5Hash(Result.Password),
                        Login = Result.Login,
                        IPAddress = FileManager.GetIP()!.ToString(),
                        IsHost = true
                    });

                    Result.Message = string.Format(Strings.User_created, Result.Login);
                    DialogResult = true;
                }
                else
                {
                    passwordBox.Password = string.Empty;
                }
            }
            else //is not local user
            {
                _fileManager.RegisterUser(new Models.UserRegistraionModel()
                {
                    PasswordHash = PasswordExtensions.MD5Hash(Result.Password),
                    Login = Result.Login,
                    IPAddress = FileManager.GetIP()!.ToString(),
                    IsHost = true
                });

                Result.Message = string.Format(Strings.User_created, Result.Login);
                DialogResult = true;
            }
        }
    }
}
