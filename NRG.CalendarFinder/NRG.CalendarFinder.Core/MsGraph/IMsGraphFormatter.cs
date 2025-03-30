using NRG.CalendarFinder.Core.CalendarFinders.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.MsGraph;
public interface IMsGraphFormatter
{
    FormattedResponse Format(OneOf<FoundResult, FoundException> e);
}