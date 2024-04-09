namespace ParallelDfs.Data;

public class Node
{
    public long Value { get; set; }
    
    public int Height { get; set; }
    
    public int Depth { get; set; }
    
    public List<Node> Children { get; set; } = [];
}
