using NRG.CalendarFinder.Core.Models;

namespace NRG.CalendarFinder.Models;

public record ProcessData
{
	public required Options Options { get; init; }
	public InputData[] Inputs { get; init; } = [];
}
