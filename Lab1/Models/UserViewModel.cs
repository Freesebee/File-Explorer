namespace Lab1.Models;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string IPAdress { get; set; }
    public bool IsBlocked { get; set; }
}
