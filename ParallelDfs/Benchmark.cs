using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ParallelDfs.Data;
using ParallelDfs.Helpers;

namespace ParallelDfs;
public class ParallelVisitorBenchmark
{
    private Tree _tree;
    private ParallelVisitor _parallelVisitor;
    private int _nodeValueToFind;

    [Params(5)] // Указывайте разные значения глубины для бенчмарка
    public int DepthLimit { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Создайте экземпляр дерева и экземпляр ParallelVisitor для бенчмарка
        _tree = TreeBuilder.CreateFullTree(new SequenceInitializer(), 20, 2);
        _parallelVisitor = new ParallelVisitor(DepthLimit);
        _nodeValueToFind = 156456; // Значение узла, которое нужно найти
    }

    [Benchmark]
    public Node? FindNodeParallel()
    {
        // Вызов метода для поиска узла в параллельном режиме
        return _parallelVisitor.FindNodeOrDefault(_tree, _nodeValueToFind);
    }

    // Добавьте другие методы бенчмарка по мере необходимости

    // Запуск бенчмарка
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<ParallelVisitorBenchmark>();
    }
}

public class SequenceVisitorBenchmark
{
    private Tree _tree;
    private SequenceVisitor _sequenceVisitor;
    private int _nodeValueToFind;

    [GlobalSetup]
    public void Setup()
    {
        // Создайте экземпляр дерева и экземпляр ParallelVisitor для бенчмарка
        _tree = TreeBuilder.CreateFullTree(new SequenceInitializer(), 20, 2);
        _sequenceVisitor = new SequenceVisitor();
        _nodeValueToFind = 156456; // Значение узла, которое нужно найти
    }

    [Benchmark]
    public Node? FindNodeSequence()
    {
        // Вызов метода для поиска узла в параллельном режиме
        return _sequenceVisitor.FindNodeOrDefault(_tree, _nodeValueToFind);
    }

    // Добавьте другие методы бенчмарка по мере необходимости

    // Запуск бенчмарка
    // public static void Main(string[] args)
    // {
    //     BenchmarkRunner.Run<SequenceVisitorBenchmark>();
    // }
}