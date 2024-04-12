namespace ParallelDfs.Helpers;

public class RandomInitializer(int minValue, int maxValue) : IInitializer
{
    private readonly Random _generator = new();
    private readonly int _minValue = minValue;
    private readonly int _maxValue = maxValue + 1;

    public int GetNextValue() => _generator.Next(_minValue, _maxValue);
}
