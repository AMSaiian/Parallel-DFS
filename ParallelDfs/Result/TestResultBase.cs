using System.Text.Json.Serialization;

namespace ParallelDfs.Result;

[JsonDerivedType(typeof(SequenceTestResult), "Sequence")]
[JsonDerivedType(typeof(ParallelTestResult), "Parallel")]
[JsonDerivedType(typeof(FullTestResult), "Full")]
public abstract class TestResultBase
{
    public int TreeHeight { get; set; }
    
    public int NodesAmount { get; set; }
    
    public int SearchedValue { get; set; }
    
    public bool MustExist { get; set; }
}
