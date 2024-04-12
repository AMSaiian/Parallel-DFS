using ParallelDfs.Data;

namespace ParallelDfs.Visitors;

public class SequenceVisitor : IVisitor
{
    public Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue)
    {
        if (tree.Root is null)
            return Task.FromResult<Node?>(default);

        Stack<Node> searchStack = new();
        searchStack.Push(tree.Root);
        
        while (searchStack.Count > 0)
        {
            Node currentNode = searchStack.Pop();

            if (currentNode.Value == nodeValue)
                return Task.FromResult<Node?>(currentNode);
            
            if (currentNode.Right is not null)
                searchStack.Push(currentNode.Right);
            
            if (currentNode.Left is not null)
                searchStack.Push(currentNode.Left);
        }

        return Task.FromResult<Node?>(default);
    }
}
