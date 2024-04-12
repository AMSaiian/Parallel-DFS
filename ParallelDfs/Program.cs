using System.Diagnostics;
using ParallelDfs.Data;
using ParallelDfs.Helpers;
using ParallelDfs.Visitors;

Tree result = TreeBuilder.CreateFullTree(new SequenceInitializer(), 24);
Console.WriteLine("Done");
var watch = Stopwatch.StartNew();
Node? foundedNode = await new ParallelVisitor(10).FindNodeOrDefault(result, 5434);
// Node? foundedNode = await new SequenceVisitor().FindNodeOrDefault(result, 5434);
watch.Stop();
Console.WriteLine(foundedNode.Value);
Console.WriteLine(watch.ElapsedMilliseconds);