using ParallelDfs.Data;

namespace ParallelDfs;

public class ParallelVisitor(int workerDepthLimit) 
{
    private volatile int _visitedCount;

    private readonly int _workerDepthLimit = workerDepthLimit;

    private volatile bool _found;

    public Node? FindNodeOrDefault(Tree tree, int nodeValue)
    {
        Node? result = ProcessSubTree(nodeValue, workerDepthLimit, tree.Root!);
        return result;
    }
    
    private Node? ProcessSubTree(int nodeValue, int depthThreshold, params Node[] subRoot)
    {
        Stack<Node> searchStack = new();
        
        for (int i = subRoot.Length - 1; i > -1; i--)
            searchStack.Push(subRoot[i]);
        
        int nextWorkerDepthThreshold = depthThreshold + _workerDepthLimit;
        List<Task<Node?>> childrenTasks = new();
        
        while (searchStack.Count > 0)
        {
            if (_found)
                return null;
            
            Node currentNode = searchStack.Pop();
            Interlocked.Increment(ref _visitedCount);
            List<Node> overThresholdNodes = new();

            if (currentNode.Value == nodeValue)
            {
                Console.WriteLine(currentNode.Value);
                _found = true;
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
                childrenTasks.Add(Task.Run(() => ProcessSubTree(nodeValue, 
                                                                nextWorkerDepthThreshold,
                                                                overThresholdNodes.ToArray())));
            }
        }
        
        Node?[] childrenResult = Task.WhenAll(childrenTasks).Result;

        return Array.Find(childrenResult, node => node is not null);
    }
}
