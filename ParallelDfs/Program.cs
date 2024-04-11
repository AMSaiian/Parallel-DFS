using System.Diagnostics;
using ParallelDfs;
using ParallelDfs.Data;
using ParallelDfs.Helpers;

// Console.WriteLine("Hello, World!");
//
Tree result = TreeBuilder.CreateFullTree(new SequenceInitializer(), 27, 2);
Console.WriteLine("Done");
var watch = Stopwatch.StartNew();
// Node? foundedNode = new ParallelVisitor(2).FindNodeOrDefault(result, (int)result.NodesAmount - 1);
Node? foundedNode = new SequenceVisitor().FindNodeOrDefault(result, (int)result.NodesAmount - 1);
Console.WriteLine(foundedNode.Value);
Console.WriteLine(watch.ElapsedMilliseconds);
// Console.WriteLine("Hello, World!");

// ParallelVisitorBenchmark.Main(null);