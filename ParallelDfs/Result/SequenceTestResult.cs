namespace ParallelDfs.Result;

public class SequenceTestResult : TestResultBase
{
    public double MeanElapsedTime { get; set; }
    
    public List<double> ElapsedTime { get; set; } = [];
}
