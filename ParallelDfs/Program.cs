using System.Text.Json;
using ParallelDfs;
using ParallelDfs.Helpers;
using ParallelDfs.Result;


int treeDepth = 20;
int searchedValue = (int)Math.Pow(2, treeDepth + 1) - 2;
int[] childTaskHeights = [12];
int[] workersAmounts = [2, 4, 8, 12];

string binDirectory = Environment.CurrentDirectory;
string projectPath = Directory.GetParent(binDirectory).Parent.Parent.FullName;
string reportPath = $"{projectPath}/Reports/{DateTime.Now.ToFileTime()}.json";

Test test = new(new SequenceInitializer(), searchedValue, treeDepth, childTaskHeights, workersAmounts);
TestResult testResult = await test.PerformTest();

var options = new JsonSerializerOptions{ WriteIndented = true };
await using FileStream createStream = File.Create(reportPath);
await JsonSerializer.SerializeAsync(createStream, testResult, options);
