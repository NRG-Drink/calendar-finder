using Microsoft.Graph.Models;
using NRG.CalendarFinder.Core.CalendarFinders.Models;
using NRG.CalendarFinder.Core.MsGraph.Models;
using OneOf;
using System.Collections.Immutable;

namespace NRG.CalendarFinder.Core.MsGraph;

public class MsGraphFormatter : IMsGraphFormatter
{
    public FormattedResponse Format(OneOf<FoundResult, FoundException> e)
        => e.Match(
            r =>
            {
                var (user, defaultCalendar, calendars) = Format(r.User, r.Calendars);
                return new FormattedResponse(r.UserIdentifier, user, defaultCalendar, [.. calendars], null);
            },
            ex =>
            {
                var error = FormatException(ex.Error);
                return new FormattedResponse(ex.UserIdentifier, null, null, null, error);
            });



    public (FormattedUser User, FormattedCalendar? DefaultCalendar, ImmutableArray<FormattedCalendar> Calendars) Format(
        User user,
        IEnumerable<Calendar> calendars
        )
    {
        var formattedCalendars = calendars.Select(FormatCalendar);
        var defaultCalendar = formattedCalendars.FirstOrDefault(e => e.IsDefault == true);
        var formattedUser = FormatUser(user, defaultCalendar);

        return (formattedUser, defaultCalendar, [.. formattedCalendars]);
    }

    public FormattedUser FormatUser(User e, FormattedCalendar? c)
        => new()
        {
            UserId = e.Id,
            DisplayName = e.DisplayName,
            PrincipalName = e.UserPrincipalName,
            Mail = e.Mail,
            DefaultCalendarId = c?.Id,
            DefaultCalendarName = c?.Name
        };

    public FormattedCalendar FormatCalendar(Calendar e)
        => new()
        {
            Id = e.Id,
            Name = e.Name,
            Owner = e.Owner?.Name,
            IsDefault = e.IsDefaultCalendar
        };

    public FormattedException? FormatException(Exception? e)
        => e is null
            ? null
            : new()
            {
                Type = e.GetType().Name,
                Message = e.Message,
                InnerException = FormatException(e.InnerException)
            };
}
