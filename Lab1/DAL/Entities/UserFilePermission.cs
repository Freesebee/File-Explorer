namespace Lab1.DAL.Entities;

public enum FilePermission
{
    Download,
    Upload,
    Notify,
}

public class UserFilePermission : AuditableEntity
{
    public Guid PermittedId { get; set; }
    public User Permitted { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public DateTime Created { get; set; }
    public Guid ModifiedById { get; set; }
    public User ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    public FilePermission Permission { get; set; }
    public Guid FileMetadataId { get; set; }
    public FileMetadata FileMetadata { get; set; }
}
