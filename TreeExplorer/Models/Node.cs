using System.Collections.Generic;

namespace TreeExplorer.Models
{
  public class Node
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string ParentName { get; set; }
    public int ParentId { get; set; }


    public List<Node> Children { get; set; } = new List<Node>();
    public Node Parent { get; set; }
    public bool IsExpanded { get; set; } = false;
    public bool IsSelected { get; set; } = false;

    public Node(int id, string name, string type, string parentName, int parentId)
    {
      Id = id;
      Name = name;
      Type = type;
      ParentName = parentName;
      ParentId = parentId;
    }

    public Node(string name, string type, string parentName, int parentId)
    {
      Name = name;
      Type = type;
      ParentName = parentName;
      ParentId = parentId;
    }

    public Node(string name, string type)
    {
      Name = name;
      Type = type;
    }

    public Node(string name)
    {
      Name = name;
    }

    public Node()
    {
    }

    public void AddChild(Node child)
    {
      Children.Add(child);
      child.Parent = this;
    }

    public void RemoveChild(Node child)
    {
      Children.Remove(child);
      child.Parent = null;
    }

    public void ClearChildren()
    {
      foreach (var child in Children)
      {
        child.Parent = null;
      }
      Children.Clear();
    }

    public override string ToString()
    {
      return $"{Name} (ID: {Id}, Type: {Type})";
    }
  }
}
