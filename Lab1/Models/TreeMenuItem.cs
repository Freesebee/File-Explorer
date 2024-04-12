using System.Collections.ObjectModel;
using System.IO;

namespace Lab1;

public class TreeMenuItem
{
    public required string Header { get; set; }
    public required string Path { get; set; }
    public bool IsDirectory { get; set; }
    public FileAttributes[] Attributes { get; set; }
    
    public ObservableCollection<TreeMenuItem> Items { get; set; } = [];

    public TreeMenuItem() { }
}
