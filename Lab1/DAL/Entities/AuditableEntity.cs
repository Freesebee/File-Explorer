using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1.DAL.Entities;

public abstract class AuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public bool IsActive { get; set; } = true;
}
