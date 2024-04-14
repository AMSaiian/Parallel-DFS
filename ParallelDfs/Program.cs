using System.Text.Json;
using ParallelDfs;
using ParallelDfs.Helpers;
using ParallelDfs.Result;

string configurationPath = "config.json";

await using FileStream configStream = File.OpenRead(configurationPath);
Configuration configuration = await JsonSerializer.DeserializeAsync<Configuration>(configStream);

Test test = new(new SequenceInitializer(), 
                configuration!.SearchedValue, 
                configuration.TreeDepth, 
                configuration.WorkIterationsAmount, 
                configuration.IdleIterationsAmount, 
                configuration.MustExist);

TestResultBase? testResult;

switch (configuration.TestVariant)
{
    case Configuration.SequenceTest:
        testResult = await test.SequenceTest();
        break;
    case Configuration.ParallelTest:
        testResult = await test.ParallelTest(configuration.ChildTasksHeights);
        break;
    case Configuration.FullTest:
        testResult = await test.FullTest(configuration.ChildTasksHeights);
        break;
    default:
        throw new ArgumentException("Invalid test scenario provided");
}

string reportPath = $"Reports/{testResult.GetType().Name}.{DateTime.Now.ToFileTime()}.json";
var options = new JsonSerializerOptions{ WriteIndented = true };
await using FileStream reportStream = File.Create(reportPath);
await JsonSerializer.SerializeAsync(reportStream, testResult, options);
