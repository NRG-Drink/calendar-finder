using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models.ODataErrors;
using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class CalendarFinderWorker(
	IHost host,
	CalendarFinderService finder,
	ProcessData processData
	)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Console.Out.WriteLineAsync($"Start Process with {processData.Options}");

		var tasks = processData.Inputs.Select(ProcessInputsAsync);
		var processed = await Task.WhenAll(tasks);

		var path = GetResultPath(processData.Options.FilePath);
		await WriteToFileAsync(path, processed);

		if (processData.Options.OpenEditor == true)
		{
			OpenVsCode(path);
		}

		await host.StopAsync(stoppingToken);
	}

	private async Task<OutputData> ProcessInputsAsync(InputData input)
	{
		var output = new OutputData() { Input = input };
		try
		{
			var user = await finder.FindUserOrThrowAsync(input.UserIdentifier);
			output = output.WithUser(user);
			var calendars = await finder.FindCalendarsOrThrowAsync(user);
			output = output.WithCalendars(calendars);
		}
		catch (ODataError oex)
		{
			output = output.WithError(oex);
		}
		catch (Exception ex)
		{
			output = output.WithError(ex);
		}
		finally
		{
			await Console.Out.WriteLineAsync(
				$"found: {output.Error is null,-5} - {input.UserIdentifier}");
		}

		return output;
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
