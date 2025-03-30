using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.CalendarFinders.Models;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.Tests.CalendarFinders.DI;
using TUnit.Assertions.AssertionBuilders.Groups;

namespace NRG.CalendarFinder.Core.Tests.CalendarFinders;
[Category("External")]
[Category("CalendarFinder")]
[CalendarFinderDI]
public class ICalendarFinderTests(
    AppSettings settings,
    ICalendarFinder calfi
    )
{
    [Test]
    [Arguments("huck")]
    [Arguments("norris")]
    [Arguments("Chuck Norris (Texas Ranger ähm)")]
    [Arguments("seschool")]
    [Arguments(".ch")]
    [Arguments(".com")]
    [Arguments("6cc27698")]
    public async Task NoUserEx(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);
        if (foundResult.Value is FoundResult f)
        {
            Assert.Fail($"Found user: {f.User.DisplayName}, {f.User.UserPrincipalName}");
        }

        var fex = await Assert.That(foundResult.Value).IsTypeOf<FoundException>().And.IsNotNull();
        var ex = await Assert.That(fex.Error).IsTypeOf<Exception>().And.IsNotNull();
        await Assert.That(ex).HasMessageContaining("no user", StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    public async Task InvalidUserEx(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);
        if (foundResult.Value is FoundResult f)
        {
            Assert.Fail($"Found user: {f.User.DisplayName}, {f.User.UserPrincipalName}");
        }

        var fex = await Assert.That(foundResult.Value).IsTypeOf<FoundException>().And.IsNotNull();
        var ex = await Assert.That(fex.Error).IsTypeOf<ArgumentException>().And.IsNotNull();
        await Assert.That(ex)
            .HasMessageContaining("null", StringComparison.OrdinalIgnoreCase)
            .And.HasMessageContaining("empty", StringComparison.OrdinalIgnoreCase)
            .And.HasMessageContaining("whitespace", StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    public async Task FindCalendarAppSetting()
    {
        foreach (var userIdentifier in settings.UserIdentifiers)
        {
            Console.WriteLine($"Try find '{userIdentifier}'");
            await FindCalendar(userIdentifier);
            Console.WriteLine();
        }
    }

    [Test]
    [Arguments("6cc27698-f450-49aa-8910-1f64bad30f92")]
    [Arguments("chuck.norris@iseschool.ch")]
    [Arguments("chuck")]
    [Arguments("Chuck.Norris")]
    public async Task FindCalendar(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);
        if (foundResult.Value is Exception ex)
        {
            Assert.Fail($"{userIdentifier} - {ex.Message}");
        }

        var found = await Assert.That(foundResult.Value).IsTypeOf<FoundResult>().And.IsNotNull();

        await Assert.That(found.UserIdentifier).IsEqualTo(userIdentifier);
        var u = await Assert.That(found.User).IsNotNull();

        var id = await Assert.That(u.Id).IsNotNull();
        var gId = AssertionGroup.For(id)
            .WithAssertion(e => e.IsNotNull())
            .And(e => e.IsEqualTo(userIdentifier, StringComparison.OrdinalIgnoreCase));

        var dn = await Assert.That(u.DisplayName).IsNotNull();
        var gDn = AssertionGroup.For(dn)
            .WithAssertion(e => e.IsNotNullOrEmpty())
            .And(e => e.Contains(userIdentifier, StringComparison.OrdinalIgnoreCase));

        var upn = await Assert.That(u.UserPrincipalName).IsNotNull();
        var gUpn = AssertionGroup.For(upn)
            .WithAssertion(e => e.IsNotNullOrEmpty())
            .And(e => e.Contains(userIdentifier, StringComparison.OrdinalIgnoreCase));

        var mail = await Assert.That(u.Mail).IsNotNull();
        var gMail = AssertionGroup.For(mail)
            .WithAssertion(e => e.IsNotNullOrEmpty())
            .And(e => e.Contains(userIdentifier, StringComparison.OrdinalIgnoreCase));

        await AssertionGroup.Assert(gId).Or(gDn).Or(gUpn).Or(gMail);

        Console.WriteLine($"Id: {u.Id}");
        Console.WriteLine($"Dn: {u.DisplayName}");
        Console.WriteLine($"Upn: {u.UserPrincipalName}");
        Console.WriteLine($"Mail: {u.Mail}");
    }
}
