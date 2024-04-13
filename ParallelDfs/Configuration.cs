using System.Text.Json.Serialization;

namespace ParallelDfs;

public class Configuration
{
    private int _treeDepth;
    
    public int IdleIterationsAmount { get; set; }
    
    public int WorkIterationsAmount { get; set; }

    public int TreeDepth
    {
        get => _treeDepth;
        set
        {
            _treeDepth = value;
            SearchedValue = (int)Math.Pow(2, _treeDepth + 1) - 2;
        }
    }

    public int[] ChildTasksHeights { get; set; } = [];

    [JsonIgnore]
    public int SearchedValue { get; private set; }
}
