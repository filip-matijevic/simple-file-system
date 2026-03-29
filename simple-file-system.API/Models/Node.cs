namespace simple_file_system.API.Models;

public enum NodeType
{
    File,
    Directory
}

public class Node
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public NodeType Type { get; set; }
    public long? ParentId { get; set; }
    public Node? Parent { get; set; }
    public ICollection<Node> Children { get; set; } = new List<Node>();
}
