using Microsoft.Graph.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public interface IUserService
{
    Task<OneOf<User, Exception>> FindUserAsync(string userIdentifier);
}
