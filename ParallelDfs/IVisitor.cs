using ParallelDfs.Data;

namespace ParallelDfs;

public interface IVisitor
{
    Task<Node?> FindNodeOrDefault(Tree tree, int nodeValue);
}
