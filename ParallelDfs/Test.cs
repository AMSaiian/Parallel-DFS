using System.Diagnostics;
using ParallelDfs.Data;
using ParallelDfs.Helpers;
using ParallelDfs.Result;
using ParallelDfs.Visitors;

namespace ParallelDfs;

public class Test(IInitializer initializer,
                      int searchedValue,
                      int treeDepth,
                      int[] childTasksHeights)
{
    private static readonly int IdleIterationsAmount = 20;
    private static readonly int WorkIterationsAmount = 10;
    
    private readonly Tree _testTree = TreeBuilder.CreateFullTree(initializer, treeDepth);
    private readonly int _searchedValue = searchedValue;
    private readonly int[] _childTasksHeights = childTasksHeights;
    private readonly IVisitor[] _visitors = [ new SequenceVisitor(), new ParallelVisitor() ];
    
    public async Task<TestResult> PerformTest()
    {
        TestResult result = new()
        {
            NodesAmount = _testTree.NodesAmount, 
            TreeHeight = _testTree.Root.Height, 
            SearchedValue = _searchedValue,
            WorkersAmount = Environment.ProcessorCount
        };

        await PerformSequenceVisitorTest(_visitors[0] as SequenceVisitor, result);
        await PerformParallelVisitorTest(_visitors[1] as ParallelVisitor, result);

        return result;
    }

    private async Task PerformParallelVisitorTest(ParallelVisitor visitor, TestResult result)
    {
        await PerformIdleRuns(visitor);

        for (int i = 0; i < WorkIterationsAmount; i++)
        {
            foreach (int childTasksHeight in _childTasksHeights)
            {
                visitor.ChildTaskHeight = childTasksHeight;
                Stopwatch timer = Stopwatch.StartNew();
                Node? resultNode = await visitor.FindNodeOrDefault(_testTree, _searchedValue);
                timer.Stop();
                double elapsedTime = timer.Elapsed.TotalMicroseconds;
                
                if (resultNode is null)
                    throw new InvalidOperationException($"Parallel visitor return null. " +
                                                        $"child task height:{childTasksHeight}");
            
                result.ParallelElapsedTime.Add(new(childTasksHeight, elapsedTime));
            }
        }
    }

    private async Task PerformSequenceVisitorTest(SequenceVisitor visitor, TestResult result)
    {
        await PerformIdleRuns(visitor);

        for (int i = 0; i < WorkIterationsAmount; i++)
        {
            Stopwatch timer = Stopwatch.StartNew();
            Node? resultNode = await visitor.FindNodeOrDefault(_testTree, _searchedValue);
            timer.Stop();
            
            if (resultNode is null)
                throw new InvalidOperationException("Sequence visitor return null");
            
            double elapsedTime = timer.Elapsed.TotalMicroseconds;
                
            result.SequenceElapsedTime.Add(elapsedTime);
        }
    }

    private async Task PerformIdleRuns(IVisitor visitor)
    {
        for (int i = 0; i < IdleIterationsAmount; i++)
            await visitor.FindNodeOrDefault(_testTree, _searchedValue);
    }
}
