namespace ParallelDfs.Result;

public class TestResult
{
    public List<long> SequenceElapsedTime { get; set; } = [];
    
    public List<ParallelCase> ParallelElapsedTime { get; set; } = [];
    
    public int TreeHeight { get; set; }
    
    public int NodesAmount { get; set; }
    
    public int SearchedValue { get; set; }
}

public record ParallelCase(int WorkersAmount, int ChildTaskHeight, long ElapsedTime);