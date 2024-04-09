using ParallelDfs.Data;

namespace ParallelDfs;

public interface IVisitor
{
    Node? FindNodeOrDefault(Tree tree, int nodeValue);
}
