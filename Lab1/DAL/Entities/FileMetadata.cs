using Lab1.Models;

namespace Lab1.DAL.Entities;

public class FileMetadata : AuditableEntity
{
    public virtual List<Operation> History { get; set; } = new();
    public string? Title { get; set; }
    
    public List<UserFilePermission> UserFilePermissions { get; set; } = new();

    public string? Subject { get; set; }
    public string? Publisher { get; set; }
    public string? Contributor { get; set; }
    public DateTime Date { get; set; }
    public string? Type { get; set; }
    public string? Format { get; set; }
    public string? Identifier { get; set; }
    public string? Source { get; set; }
    public string? Language { get; set; }
    public string? Relation { get; set; }
    public string? Coverage { get; set; }
    public string? Rights { get; set; }
    public string? Description { get; set; }
}
