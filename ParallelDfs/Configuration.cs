using System.Text.Json.Serialization;

namespace ParallelDfs;

public class Configuration
{
    public const int SequenceTest = 0;
    
    public const int ParallelTest = 1;

    public const int FullTest = 2;
    
    public int IdleIterationsAmount { get; set; }
    
    public int WorkIterationsAmount { get; set; }

    public int TreeDepth { get; set; }

    public int[] ChildTasksHeights { get; set; } = [];
    
    public int SearchedValue { get; set; }
    
    public bool MustExist { get; set; }
    
    public int TestVariant { get; set; }
}
