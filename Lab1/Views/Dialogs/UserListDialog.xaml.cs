using Lab1.BLL;
using Lab1.DAL.Entities;
using Lab1.Extensions;
using Lab1.Models;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Lab1.Views.Dialogs;

/// <summary>
/// Interaction logic for UserListDialog.xaml
/// </summary>
public partial class UserListDialog : Window
{
    public class UserListDialogResult
    {
        public List<UserViewModel> Users { get; set; } = new();
    }

    public UserListDialogResult? Result { get; set; }

    public List<UserViewModel> UserDataSource = new();

    public UserListDialog(IEnumerable<User> users)
    {
        InitializeComponent();

        UserDataSource = users.Select(x => new UserViewModel
        {
            Id = x.Id,
            IPAdress = x.IPAddress,
            IsBlocked = !x.IsActive,
            Login = x.Login,
        }).ToList();

        userDataGrid.ItemsSource = UserDataSource;
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;

        Close();
    }

    private void Button_Click_Ok(object sender, RoutedEventArgs e)
    {
        Result = new() { Users = UserDataSource };
        
        DialogResult = true;        
    }
}
