using ParallelDfs;
using ParallelDfs.Data;
using ParallelDfs.Helpers;

Console.WriteLine("Hello, World!");

Tree result = TreeBuilder.CreateFullTree(new SequenceInitializer(), 20, 2);
Node? foundedNode = new ParallelVisitor(5).FindNodeOrDefault(result, 1800233);
// Node? foundedNode = new SequenceVisitor().FindNodeOrDefault(result, 1800233);
Console.WriteLine("Hello, World!");