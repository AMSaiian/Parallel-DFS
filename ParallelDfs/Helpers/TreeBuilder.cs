using System.Collections;
using ParallelDfs.Data;

namespace ParallelDfs.Helpers;

public static class TreeBuilder
{
    public static Tree CreateFullTree(IInitializer initializer,
                                      int depth)
    {
        if (depth < 1)
            throw new ArgumentException("Depth of tree must be greater than 1", nameof(depth));
        
        Queue<Node> initQueue = new();
        
        Tree newTree = new();
        newTree.Root = new()
        {
            Depth = 0,
            Height = depth,
            Value = initializer.GetNextValue()
        };
        int nodesAmount = 1;
        
        initQueue.Enqueue(newTree.Root);
        
        do
        {
            Node currentNode = initQueue.Dequeue();

            currentNode.Left = new()
            {
                Depth = currentNode.Depth + 1,
                Height = currentNode.Height - 1,
                Value = initializer.GetNextValue()
            };
            currentNode.Right = new()
            {
                Depth = currentNode.Depth + 1,
                Height = currentNode.Height - 1,
                Value = initializer.GetNextValue()
            };
            
            nodesAmount += 2;

            if (currentNode.Depth < depth - 1)
            {
                initQueue.Enqueue(currentNode.Left);
                initQueue.Enqueue(currentNode.Right);
            }
        } while (initQueue.Count > 0);

        newTree.NodesAmount = nodesAmount;
        
        return newTree;
    }
}
