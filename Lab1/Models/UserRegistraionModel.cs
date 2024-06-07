namespace Lab1.Models;

public class UserRegistraionModel
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string IPAddress { get; set; }
    public bool IsHost { get; set; }
}
