using System.Collections.Concurrent;
using ParallelDfs.Data;
using Nito.Collections;

namespace ParallelDfs;

public class ParallelVisitor(int workerDepthLimit) 
{
    // private volatile int _visitedCount;
    //
    // private readonly int _workerDepthLimit = workerDepthLimit;

    private volatile bool _found;

    private readonly ReaderWriterLock _locker = new();

    public Node? FindNodeOrDefault(Tree tree, int nodeValue)
    {
        Node? subResult = ProcessSubTree(nodeValue, tree.Root!);
        
        return subResult;
    }
    
    private Node? ProcessSubTree(int nodeValue, params Node[] subRoot)
    {
        Deque<Node> searchDeque = new();
        
        for (int i = subRoot.Length - 1; i > -1; i--)
            searchDeque.AddToFront(subRoot[i]);

        List<Task<Node?>> subTasks = new();
        while (searchDeque.Count > 0)
        {   
            // _locker.AcquireReaderLock(int.MaxValue);
            if (_found)
            {
                if (subTasks.Count > 0)
                {
                    Task<Node?[]> subResults = Task.WhenAll(subTasks);
                    subResults.Wait();
            
                    return Array.Find(subResults.Result, sr => sr is not null);
                }
            }
            // _locker.ReleaseReaderLock();
            
            Node currentNode = searchDeque.RemoveFromFront();

            if (currentNode.Value == nodeValue)
            {
                // _locker.AcquireWriterLock(int.MaxValue);
                _found = true;
                // _locker.ReleaseWriterLock();
                Console.WriteLine(currentNode.Value);
                return currentNode;
            }

            if (currentNode.Children.Count > 0)
            {
                for (int i = currentNode.Children.Count - 1; i > -1; i--)
                    searchDeque.AddToFront(currentNode.Children[i]);
            }
            else if (searchDeque.Count > 0 
                  && searchDeque[^1].Height > 20)
            {
                Node highestNeighbour = searchDeque.RemoveFromBack();
                
                subTasks.Add(Task.Run(() => ProcessSubTree(nodeValue,
                                                           highestNeighbour)));
            }
        }

        if (subTasks.Count > 0)
        {
            Task<Node?[]> subResults = Task.WhenAll(subTasks);
            subResults.Wait();
            
            return Array.Find(subResults.Result, sr => sr is not null);
        }
        
        return null;
    }
}
