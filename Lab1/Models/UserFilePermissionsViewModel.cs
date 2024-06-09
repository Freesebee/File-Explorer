namespace Lab1.Models;

public class UserFilePermissionsViewModel
{
    public Guid FileMetadataId { get; set; }
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanBeNotified { get; set; }
    public Guid UserId { get; set; }
    public string User { get; set; }
}
