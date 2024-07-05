using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.Core;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.MsGraphFactories;
using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class CalendarFinderWorker(
	IHost host,
	IMsGraphClientFactory graphClientFactory,
	ProcessData processData
	)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Console.Out.WriteLineAsync($"Start Process with {processData.Options}");

		var graph = graphClientFactory.GetClient("GraphClient");
		var finder = new CalendarFinderService(graph);
		var tasks = processData.Inputs.Select(finder.ProcessInputAsync);
		var processed = await Task.WhenAll(tasks);

		var path = GetResultPath(processData.Options.FilePath);
		await WriteToFileAsync(path, processed);

		if (processData.Options.OpenEditor == true)
		{
			OpenVsCode(path);
		}

		await host.StopAsync(stoppingToken);
	}

	private static async Task WriteToFileAsync(string path, IEnumerable<OutputData> output)
	{
		var text = JsonSerializer.Serialize(output, options: new() { WriteIndented = true });
		await File.WriteAllTextAsync(path, text);
		await Console.Out.WriteLineAsync($"Wrote output data to file {path}.");
	}

	private static string GetResultPath(string inputPath)
	{
		var name = Path.GetFileNameWithoutExtension(inputPath);
		var dir = Path.GetPathRoot(inputPath)
			?? throw new ArgumentNullException(
				$"No root directory for file {inputPath} found. " +
				$"Output file could not be written."
			);
		var path = Path.Combine(dir, $"{name}.result.json");
		return path;
	}

	private static void OpenVsCode(string path)
	{
		var process = new System.Diagnostics.Process()
		{
			StartInfo = new()
			{
				UseShellExecute = true,
				FileName = "code",
				Arguments = path
			}
		};
		process.Start();
	}
}
