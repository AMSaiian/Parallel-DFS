using System.Text.Json;
using ParallelDfs;
using ParallelDfs.Helpers;
using ParallelDfs.Result;


string binDirectory = Environment.CurrentDirectory;
string projectPath = Directory.GetParent(binDirectory).Parent.Parent.FullName;
string configurationPath = $"{projectPath}/config.json";

await using FileStream configStream = File.OpenRead(configurationPath);
Configuration configuration = await JsonSerializer.DeserializeAsync<Configuration>(configStream);

Test test = new(new SequenceInitializer(), 
                configuration!.SearchedValue, 
                configuration.TreeDepth, 
                configuration.ChildTasksHeights, 
                configuration.WorkIterationsAmount, 
                configuration.IdleIterationsAmount);

TestResult testResult = await test.PerformTest();

string reportPath = $"{projectPath}/Reports/{DateTime.Now.ToFileTime()}.json";
var options = new JsonSerializerOptions{ WriteIndented = true };
await using FileStream reportStream = File.Create(reportPath);
await JsonSerializer.SerializeAsync(reportStream, testResult, options);
