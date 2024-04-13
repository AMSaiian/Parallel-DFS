using Nito.Collections;
using ParallelDfs.Data;

namespace ParallelDfs.Visitors;

public class ParallelVisitor : IVisitor
{
    private volatile bool _found;
    private volatile Node? _result;
    private readonly object _locker = new();

    public int ChildTaskHeight { get; set; }
    
    public async Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue)
    {
        _result = default;
        _found = false;
        
        await ProcessSubTree(nodeValue, tree.Root!);
        
        return _result;
    }
    
    private async Task ProcessSubTree(int nodeValue, Node subRoot)
    {
        if (_found)
            return;
            
        Deque<Node> searchDeque = new();
        
        searchDeque.AddToFront(subRoot);

        List<Task>? subTasks = null;
        
        while (searchDeque.Count > 0 
            && !_found)
        {
            Node currentNode = searchDeque.RemoveFromFront();

            if (currentNode.Value == nodeValue)
            {
                lock (_locker)
                {
                    if (!_found)
                    {
                        _found = true;
                        _result = currentNode;
                    }
                }
                break;
            }
            
            if (currentNode.Right is not null)
                searchDeque.AddToFront(currentNode.Right);
            
            if (currentNode.Left is not null)
                searchDeque.AddToFront(currentNode.Left);
            
            if (searchDeque.Count > 0
                && searchDeque[^1].Height > ChildTaskHeight
                && !_found)
            {
                subTasks ??= new();
                
                Node highestNeighbour = searchDeque.RemoveFromBack();
                
                subTasks.Add(Task.Run(() => ProcessSubTree(nodeValue, 
                                                           highestNeighbour)));
            }
        }

        if (subTasks is not null)
        { 
            await Task.WhenAll(subTasks);
        }
    }
}
