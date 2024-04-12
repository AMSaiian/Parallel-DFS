namespace ParallelDfs.Data;

public class Node
{
    public int Value { get; init; }
    
    public int Height { get; set; }
    
    public int Depth { get; set; }
    
    public Node? Left { get; set; }
    
    public Node? Right { get; set; }
}
