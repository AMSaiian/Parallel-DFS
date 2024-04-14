namespace ParallelDfs.Result;

public class ParallelTestResult : TestResultBase
{
    public List<ParallelCase> ElapsedTime { get; set; } = [];
    
    public int WorkersAmount { get; set; }
}

public record ParallelCase(int ChildTaskHeight, double ElapsedTime);