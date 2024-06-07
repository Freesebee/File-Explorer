using System.Windows;

namespace Lab1.Views.Dialogs;

/// <summary>
/// Interaction logic for SignInDialog.xaml
/// </summary>
public partial class SignInDialog : Window
{
    public string Login { get; set; }
    public string Password { get; set; }

    public SignInDialog()
    {
        InitializeComponent();
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Button_Click_Ok(object sender, RoutedEventArgs e)
    {
        Login = loginTextBox.Text;
        Password = passwordTextBox.Text;

        DialogResult = true;
    }
}
