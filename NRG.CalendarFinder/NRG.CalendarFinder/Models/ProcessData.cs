using System.Text.Json.Serialization;

namespace NRG.CalendarFinder.Models;

public record ProcessData
{
    public required Options Options { get; init; }
    public InputData[] Inputs { get; init; } = [];
}

public record InputData
{
    public required string UserIdentifier { get; init; }
}

public record OutputData
{
    public required InputData Input { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MyUser? User { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MyCalendar[]? Calendars { get; init; }
}

public record MyUser
{
    public string? DisplayName { get; init; }
    public string? PrincipalName { get; init; }
    public string? Mail { get; init; }
    public string? UserId { get; init; }
    public string? CalendarId { get; init; }
}

public record MyCalendar
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Owner { get; init; }
    public bool? IsDefault { get; init; }
}
