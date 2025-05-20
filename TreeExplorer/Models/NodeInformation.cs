namespace TreeExplorer.Models
{
  public class NodeInformation
  {
    public string NodeName { get; set; }
    public int NodeId { get; set; }

    public NodeInformation()
    {
    }

    public NodeInformation(string nodeName, int nodeId)
    {
      NodeName = nodeName;
      NodeId = nodeId;
    }

    public override string ToString()
    {
      return $"{NodeName} (ID: {NodeId})";
    }
  }
}
