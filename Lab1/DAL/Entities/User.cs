namespace Lab1.DAL.Entities;

public class User : AuditableEntity
{
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public bool IsHost { get; set; }
    public string IPAddress { get; set; }
    //public virtual List<UserFilePermission> OwnedPermissions { get; set; } = new();
    //public virtual List<UserFilePermission> GrantedPermissions { get; set; } = new();
    //public virtual List<Operation> Operations { get; set; } = new();
}
