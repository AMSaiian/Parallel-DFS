using ParallelDfs.Data;

namespace ParallelDfs;

public class SequenceVisitor : IVisitor
{
    private int _visitedCount;

    public Node? FindNodeOrDefault(Tree tree, int nodeValue)
    {
        _visitedCount = 0;

        if (tree.Root is null)
            return default;

        Stack<Node> searchStack = new();
        searchStack.Push(tree.Root);
        
        while (searchStack.Count > 0)
        {
            Node currentNode = searchStack.Pop();
            _visitedCount++;

            if (currentNode.Value == nodeValue)
                return currentNode;

            for (int i = currentNode.Children.Count - 1; i > -1; i--)
                searchStack.Push(currentNode.Children[i]);
        }

        return default;
    }
}
