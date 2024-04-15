namespace ParallelDfs.Result;

public class FullTestResult : TestResultBase
{
    public double SequenceMeanElapsedTime { get; set; }

    public List<ParallelCase> ParallelMeanElapsedTime { get; set; } = [];
    
    public int WorkersAmount { get; set; }
    
    public List<double> SequenceElapsedTime { get; set; } = [];
    
    public List<ParallelCase> ParallelElapsedTime { get; set; } = [];
}
