using Microsoft.Graph;

namespace NRG.CalendarFinder.MsGraphFactories;

public interface IMsGraphClientFactory
{
	public GraphServiceClient GetClient(string name);
}
