using Microsoft.Graph;

namespace NRG.CalendarFinder.Core.MsGraphFactories;

public interface IMsGraphClientFactory
{
	public GraphServiceClient GetClient(string name);
}
