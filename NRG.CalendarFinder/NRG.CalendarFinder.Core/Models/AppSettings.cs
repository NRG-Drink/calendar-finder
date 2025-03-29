using NRG.CalendarFinder.Core.MsGraph.Models;

namespace NRG.CalendarFinder.Core.Models;

public record AppSettings(MsGraphCredential MsGraphCredential, string[] UserIdentifiers);
