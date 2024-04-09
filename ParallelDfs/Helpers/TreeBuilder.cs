using System.Collections;
using ParallelDfs.Data;

namespace ParallelDfs.Helpers;

public static class TreeBuilder
{
    public static Tree CreateFullTree(IInitializer initializer,
                                      int depth,
                                      int childrenAmount)
    {
        if (depth < 1)
            throw new ArgumentException("Depth of tree must be greater than 1", nameof(depth));
        if (childrenAmount < 2)
            throw new ArgumentException("Children amount must be greater or equal 2", nameof(childrenAmount));
        
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

            for (int i = 0; i < childrenAmount; i++)
            {
                Node newChildren = new()
                {
                    Depth = currentNode.Depth + 1,
                    Height = currentNode.Height - 1,
                    Value = initializer.GetNextValue()
                };

                currentNode.Children.Add(newChildren);
                nodesAmount++;
            }

            if (currentNode.Depth < depth - 1)
            {
                foreach (Node children in currentNode.Children)
                {
                    initQueue.Enqueue(children);
                }
            }
        } while (initQueue.Count > 0);

        newTree.NodesAmount = nodesAmount;
        
        return newTree;
    }
}
