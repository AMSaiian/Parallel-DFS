using Nito.Collections;
using ParallelDfs.Data;

namespace ParallelDfs.Visitors;

public class ParallelVisitor : IVisitor
{
    private int _workersAmount;
    private readonly int _childTaskHeight = 10;
    private volatile bool _found;
    private volatile Node? _result;
    private readonly object _locker = new();

    public int WorkersAmount
    {
        get => _workersAmount;
        set
        {
            ThreadPool.GetMinThreads(out int _, out int minIoc);
            bool minResult = ThreadPool.SetMinThreads(value, minIoc);
            
            if (!minResult)
                throw new InvalidOperationException($"Can't set workers amount with value {value}");
            
            _workersAmount = value;
        }
    }

    public ParallelVisitor()
    {
        ThreadPool.GetMinThreads(out int workersAmount, out _);
        _workersAmount = workersAmount;
    }

    public ParallelVisitor(int childTaskHeight, int workersAmount)
    {
        _childTaskHeight = childTaskHeight;
        WorkersAmount = workersAmount;
    }

    public async Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue)
    {
        _result = default;
        _found = false;
        
        await ProcessSubTree(nodeValue, tree.Root!);
        
        return _result;
    }
    
    private async Task ProcessSubTree(int nodeValue, Node subRoot)
    {
        Deque<Node> searchDeque = new();
        
        searchDeque.AddToFront(subRoot);

        List<Task> subTasks = new();
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
                && searchDeque[^1].Height > _childTaskHeight
                && !_found)
            {
                Node highestNeighbour = searchDeque.RemoveFromBack();
                
                subTasks.Add(Task.Run(() => ProcessSubTree(nodeValue, 
                                                           highestNeighbour)));
            }
        }

        if (subTasks.Count > 0)
        { 
            await Task.WhenAll(subTasks);
        }
    }
}
