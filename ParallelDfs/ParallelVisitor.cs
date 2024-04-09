using ParallelDfs.Data;

namespace ParallelDfs;

public class ParallelVisitor(int workerDepthLimit) : IVisitor
{
    private volatile int _visitedCount;

    private readonly int _workerDepthLimit = workerDepthLimit;

    public Node? FindNodeOrDefault(Tree tree, int nodeValue)
    {
        CancellationTokenSource cts = new();
        CancellationToken ct = cts.Token;
        Node? result = ProcessSubTree(nodeValue, workerDepthLimit, ct, tree.Root!);
        cts.Cancel();
        return result;
    }
    
    private Node? ProcessSubTree(int nodeValue, int depthThreshold, CancellationToken outCt, params Node[] subRoot)
    {
        using CancellationTokenSource cts = new();
        CancellationToken innerCt = cts.Token;
        
        Stack<Node> searchStack = new();
        foreach (Node node in subRoot)
            searchStack.Push(node);
        
        int nextWorkerDepthThreshold = depthThreshold + _workerDepthLimit;
        List<Task<Node?>> childrenTasks = new();
        List<Node> overThresholdNodes = new();
        
        while (searchStack.Count > 0)
        {
            if (outCt.IsCancellationRequested)
                return default;
            
            Node currentNode = searchStack.Pop();
            Interlocked.Increment(ref _visitedCount);

            if (currentNode.Value == nodeValue)
            {
                cts.Cancel();
                return currentNode;
            }
            
            for (int i = currentNode.Children.Count - 1; i > -1; i--)
                searchStack.Push(currentNode.Children[i]);
            
            while (searchStack.TryPeek(out Node? overThresholdNode) 
                && overThresholdNode.Depth > depthThreshold)
            {
                overThresholdNode = searchStack.Pop();
                overThresholdNodes.Add(overThresholdNode);
            }

            if (overThresholdNodes.Count > 0)
            {
                childrenTasks.Add(Task.Run(() => 
                                            ProcessSubTree(nodeValue, 
                                                           nextWorkerDepthThreshold,
                                                           innerCt,
                                                           overThresholdNodes.ToArray()), innerCt));
            }
        }
        
        Node?[] childrenResult = Task.WhenAll(childrenTasks).Result;

        return Array.Find(childrenResult, node => node is not null);
    }
}
