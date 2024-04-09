namespace ParallelDfs.Helpers;

public class RandomInitializer(int minValue, int maxValue) : IInitializer
{
    private Random _generator = new();
    private int _minValue = minValue;
    private int _maxValue = maxValue + 1;

    public int GetNextValue() => _generator.Next(_minValue, _maxValue);
}
