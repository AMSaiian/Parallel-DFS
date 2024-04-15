using System.Diagnostics;
using ParallelDfs.Data;
using ParallelDfs.Helpers;
using ParallelDfs.Result;
using ParallelDfs.Visitors;

namespace ParallelDfs;

public class Test(IInitializer initializer,
                  int searchedValue,
                  int treeDepth,
                  int workIterationsAmount,
                  int idleIterationsAmount,
                  bool mustExist,
                  bool withConsoleOutput)
{
    private readonly int _workIterationsAmount = workIterationsAmount;
    private readonly int _idleIterationsAmount = idleIterationsAmount;
    private readonly Tree _testTree = TreeBuilder.CreateFullTree(initializer, treeDepth);
    private readonly int _searchedValue = searchedValue;
    private readonly bool _mustExist = mustExist;
    private readonly bool _withConsoleOutput = withConsoleOutput;
    private readonly IVisitor[] _visitors = [ new SequenceVisitor(), new ParallelVisitor() ];
    
    public async Task<FullTestResult> FullTest(int[] childTasksHeights)
    {
        FullTestResult result = new()
        {
            NodesAmount = _testTree.NodesAmount, 
            TreeHeight = _testTree.Root.Height, 
            SearchedValue = _searchedValue,
            MustExist = _mustExist,
            WorkersAmount = Environment.ProcessorCount
        };

        SequenceTestResult sequenceResult = 
            await PerformSequenceVisitorTest(_visitors[0] as SequenceVisitor);
        
        ParallelTestResult parallelResult = 
            await PerformParallelVisitorTest(_visitors[1] as ParallelVisitor, childTasksHeights);

        result.ParallelElapsedTime = parallelResult.ElapsedTime;
        result.ParallelMeanElapsedTime = parallelResult.MeanElapsedTime;
        
        result.SequenceElapsedTime = sequenceResult.ElapsedTime;
        result.SequenceMeanElapsedTime = sequenceResult.MeanElapsedTime;
        
        return result;
    }

    public async Task<SequenceTestResult> SequenceTest()
    {
        return await PerformSequenceVisitorTest(_visitors[0] as SequenceVisitor);
    }

    public async Task<ParallelTestResult> ParallelTest(int[] childTasksHeights)
    {
        return await PerformParallelVisitorTest(_visitors[1] as ParallelVisitor, childTasksHeights);
    }

    private async Task<ParallelTestResult> PerformParallelVisitorTest(ParallelVisitor visitor, 
                                                                      int[] childTasksHeights)
    {
        await PerformIdleRuns(visitor);

        ParallelTestResult result = new()
        {
            SearchedValue = _searchedValue,
            NodesAmount = _testTree.NodesAmount,
            TreeHeight = _testTree.Root!.Height,
            MustExist = _mustExist,
            WorkersAmount = Environment.ProcessorCount
        };

        for (int i = 0; i < _workIterationsAmount; i++)
        {
            foreach (int childTasksHeight in childTasksHeights)
            {
                visitor.ChildTaskHeight = childTasksHeight;
                Stopwatch timer = Stopwatch.StartNew();
                Node? resultNode = await visitor.FindNodeOrDefault(_testTree, _searchedValue);
                timer.Stop();
                double elapsedTime = timer.Elapsed.TotalMicroseconds;
                
                if (!await IsResultCorrect(resultNode))
                    throw new InvalidOperationException($"Parallel visitor return invalid result. " +
                                                        $"child task height:{childTasksHeight}");
                
                switch (_withConsoleOutput)
                {
                    case true when resultNode is not null:
                        Console.WriteLine("Parallel DFS found: " + resultNode);
                        break;
                    case true when resultNode is null:
                        Console.WriteLine($"Parallel DFS doesn't found node with value {_searchedValue} as expected");
                        break;
                }
                
                result.ElapsedTime.Add(new(childTasksHeight, elapsedTime));
            }
        }

        result.MeanElapsedTime = result.ElapsedTime
                                       .GroupBy(execCase => execCase.ChildTaskHeight)
                                       .Select(group => new ParallelCase(group.Key, 
                                                                         group.Average(elem => elem.ElapsedTime)))
                                       .ToList();

        return result;
    }

    private async Task<SequenceTestResult> PerformSequenceVisitorTest(SequenceVisitor visitor)
    {
        await PerformIdleRuns(visitor);
        
        SequenceTestResult result = new()
        {
            SearchedValue = _searchedValue,
            NodesAmount = _testTree.NodesAmount,
            TreeHeight = _testTree.Root!.Height,
            MustExist = _mustExist
        };

        for (int i = 0; i < _workIterationsAmount; i++)
        {
            Stopwatch timer = Stopwatch.StartNew();
            Node? resultNode = await visitor.FindNodeOrDefault(_testTree, _searchedValue);
            timer.Stop();
            
            if (!await IsResultCorrect(resultNode))
                throw new InvalidOperationException("Sequence visitor return invalid result");
            
            switch (_withConsoleOutput)
            {
                case true when resultNode is not null:
                    Console.WriteLine("Sequence DFS found: " + resultNode);
                    break;
                case true when resultNode is null:
                    Console.WriteLine($"Sequence DFS doesn't found node with value {_searchedValue} as expected");
                    break;
            }
            
            double elapsedTime = timer.Elapsed.TotalMicroseconds;
                
            result.ElapsedTime.Add(elapsedTime);
        }

        result.MeanElapsedTime = result.ElapsedTime.Average();
        
        return result;
    }

    private async Task PerformIdleRuns(IVisitor visitor)
    {
        for (int i = 0; i < _idleIterationsAmount; i++)
            await visitor.FindNodeOrDefault(_testTree, _searchedValue);
    }

    private Task<bool> IsResultCorrect(Node? resultNode)
    {
        bool result = _mustExist == resultNode is not null;

        return Task.FromResult(result);
    }
}
