namespace Lab1.DAL.Entities;

public class Operation : AuditableEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string OperationType { get; internal set; }
}
