namespace ParallelDfs.Helpers;

public class SequenceInitializer : IInitializer
{
    private int _currentValue = 0;

    public int GetNextValue() => _currentValue++;
}
