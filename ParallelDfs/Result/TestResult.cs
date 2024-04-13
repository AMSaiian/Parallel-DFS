namespace ParallelDfs.Result;

public class TestResult
{
    public List<double> SequenceElapsedTime { get; set; } = [];
    
    public List<ParallelCase> ParallelElapsedTime { get; set; } = [];
    
    public int TreeHeight { get; set; }
    
    public int NodesAmount { get; set; }
    
    public int SearchedValue { get; set; }
    
    public int WorkersAmount { get; set; }
}

public record ParallelCase(int ChildTaskHeight, double ElapsedTime);