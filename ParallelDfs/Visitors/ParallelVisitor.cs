using Nito.Collections;
using ParallelDfs.Data;

namespace ParallelDfs.Visitors;

public class ParallelVisitor(int childTaskHeight) : IVisitor
{
    private volatile bool _found;
    private readonly int _childTaskHeight = childTaskHeight;
    private readonly object _locker = new();

    public async Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue)
    {
        Node? subResult = await ProcessSubTree(nodeValue, tree.Root!);
        
        return subResult;
    }
    
    private async Task<Node?> ProcessSubTree(int nodeValue, Node subRoot)
    {
        Deque<Node> searchDeque = new();
        
        searchDeque.AddToFront(subRoot);

        List<Task<Node?>> subTasks = new();
        while (searchDeque.Count > 0)
        {
            switch (_found)
            {
                case true when subTasks.Count > 0: 
                    Node?[] subResults = await Task.WhenAll(subTasks); 
                    return Array.Find(subResults, sr => sr is not null);
                
                case true:
                    return default;
            }
            
            Node currentNode = searchDeque.RemoveFromFront();

            if (currentNode.Value == nodeValue)
            {
                lock (_locker)
                {
                    if (!_found)
                    {
                        _found = true;
                        return currentNode;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
            if (currentNode.Right is not null)
                searchDeque.AddToFront(currentNode.Right);
            
            if (currentNode.Left is not null)
                searchDeque.AddToFront(currentNode.Left);
            
            while (searchDeque.Count > 0
             && searchDeque[^1].Height > _childTaskHeight)
            {
                Node highestNeighbour = searchDeque.RemoveFromBack();
                
                subTasks.Add(Task.Run(() => ProcessSubTree(nodeValue, 
                                                           highestNeighbour)));
            }
        }

        if (subTasks.Count > 0)
        {
            Node?[] subResults = await Task.WhenAll(subTasks);
            
            return Array.Find(subResults, sr => sr is not null);
        }
        
        return null;
    }
}
