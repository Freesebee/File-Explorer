namespace Lab1.DAL.Entities;

public class IPAddress : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Address { get; set; }
}
