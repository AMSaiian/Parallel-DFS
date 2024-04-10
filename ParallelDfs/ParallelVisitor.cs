using ParallelDfs.Data;
using Nito.Collections;

namespace ParallelDfs;

public class ParallelVisitor(int workerDepthLimit) 
{
    private volatile int _visitedCount;

    private readonly int _workerDepthLimit = workerDepthLimit;

    private volatile bool _found;

    public async Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue)
    {
        Node? result = await ProcessSubTree(nodeValue, _workerDepthLimit, tree.Root!);
        return result;
    }
    
    private async Task<Node?> ProcessSubTree(int nodeValue, int depthThreshold, params Node[] subRoot)
    {
        Deque<Node> searchDeque = new();
        
        for (int i = subRoot.Length - 1; i > -1; i--)
            searchDeque.AddToFront(subRoot[i]);
        
        List<Task<Node?>> childrenTasks = new();
        
        while (searchDeque.Count > 0)
        {
            if (_found)
                return null;
            
            Node currentNode = searchDeque.RemoveFromFront();
            Interlocked.Increment(ref _visitedCount);

            if (currentNode.Value == nodeValue)
            {
                _found = true;
                Console.WriteLine(currentNode.Value);
                return currentNode;
            }

            if (currentNode.Children.Count > 0)
            {
                for (int i = currentNode.Children.Count - 1; i > -1; i--)
                    searchDeque.AddToFront(currentNode.Children[i]);
            }
            else if (searchDeque.Count > 0 
                  && searchDeque[^1].Height - searchDeque[^1].Depth > 18)
            {
                Node highestNeighbour = searchDeque.RemoveFromBack();
                int nextWorkerDepthThreshold = highestNeighbour.Depth + _workerDepthLimit;
                
                childrenTasks.Add(Task.Run(() => ProcessSubTree(nodeValue,
                                                                nextWorkerDepthThreshold,
                                                                highestNeighbour)));
            }
        }

        Node?[] results = await Task.WhenAll(childrenTasks);

        Node? result = Array.Find(results, r => r is not null);

        return result;
    }
}
