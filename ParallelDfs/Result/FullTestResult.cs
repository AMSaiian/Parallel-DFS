namespace ParallelDfs.Result;

public class FullTestResult : TestResultBase
{
    public List<double> SequenceElapsedTime { get; set; } = [];
    
    public List<ParallelCase> ParallelElapsedTime { get; set; } = [];
    
    public int WorkersAmount { get; set; }
}
