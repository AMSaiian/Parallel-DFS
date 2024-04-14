namespace ParallelDfs.Data;

public class Node
{
    public int Value { get; init; }
    
    public int Height { get; set; }
    
    public int Depth { get; set; }
    
    public Node? Left { get; set; }
    
    public Node? Right { get; set; }

    public override string ToString()
    {
        return $"Node with value: {Value} - height: {Height} - depth: {Depth}";
    }
}
