using System.Collections.ObjectModel;

namespace TreeExplorer.Models
{
  public class TreeNode
  {
    public string Name { get; set; }
    public ObservableCollection<TreeNode> Children { get; set; }

    public TreeNode()
    {
      Children = new ObservableCollection<TreeNode>();
    }

    public TreeNode(string name) : this()
    {
      Name = name;
    }

    public void AddChild(TreeNode child)
    {
      Children.Add(child);
    }

    public void AddChildren(System.Collections.Generic.IEnumerable<TreeNode> children)
    {
      foreach (var child in children)
      {
        Children.Add(child);
      }
    }
  }
}