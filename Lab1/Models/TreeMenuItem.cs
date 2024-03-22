using System.Collections.ObjectModel;
using System.IO;

namespace Lab1;

public class TreeMenuItem
{
    public required string Header { get; set; }
    public required string Path { get; set; }
    public bool IsDirectory { get; set; }
    public FileAttributes[] Attributes { get; set; }
    public string GetRashString
    {
        get
        {
            var text = string.Empty;

            text += Attributes.Contains(FileAttributes.ReadOnly) ? "r" : "-"; 
            text += Attributes.Contains(FileAttributes.Archive) ? "a" : "-"; 
            text += Attributes.Contains(FileAttributes.Hidden) ? "h" : "-"; 
            text += Attributes.Contains(FileAttributes.System) ? "s" : "-"; 

            return text;
        }
    }

    public ObservableCollection<TreeMenuItem> Items { get; set; } = [];

    public TreeMenuItem() { }
}
