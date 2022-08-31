using FluentAssertions;
using InfoTrack.GoogleRank.Services;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace InfoTrack.Google.Tests.Services;

[TestFixture]
public class DirtyHtmlScraperTests
{
    private IHtmlScraper _scraper;

    [SetUp]
    public void Setup()
    {
        _scraper = new CheaterHtmlScraper();
    }

    [Test, Ignore("Right idea, not MVP")]
    public async Task ReturnsNullWhenElementCantBeFound()
    {
        await using var stream = ReadTop100EfilingIntegrationSearchPage();

        var result = _scraper.SelectElementsWithCssSelector(stream, NullLogger.Instance);

        result.Should().BeNull();
    }

    [Test]
    public async Task LogsSkippingToSearchDiv()
    {
        var loggerSpy = new LoggerSpy();
        await using var stream = ReadTop100EfilingIntegrationSearchPage();

        _ = await _scraper.SelectElementsWithCssSelector(stream, loggerSpy);

        var debugEntries = loggerSpy.Entries.Where(x => x.LogLevel == LogLevel.Debug)
            .Select(x => x.Message.ToLower())
            .ToArray();
        debugEntries.Should().ContainEquivalentOf("found response div");
        debugEntries.Should().ContainEquivalentOf("found search results div");
    }

    [Test]
    public async Task ReturnsStringArrayWithSpecificClass()
    {
        await using var stream = ReadTop100EfilingIntegrationSearchPage();

        var result = await _scraper.SelectElementsWithCssSelector(stream, NullLogger.Instance);

        result.Should().HaveCount(95);
        result[0].Should().BeEquivalentTo(new { index = 0, HRefValue = "https://www.g2.com/categories/e-filing" });
        result[94].Should().BeEquivalentTo(new { index = 94, HRefValue = "https://www.lsc.gov/sites/default/files/Best-Pratices.pdf" });
    }

    private Stream ReadTop100EfilingIntegrationSearchPage()
    {
        const string FileName = "Top100EfilingIntegrationSearchPage.html";
        var path = Directory.EnumerateFiles(TestContext.CurrentContext.TestDirectory, FileName, SearchOption.AllDirectories).FirstOrDefault();
        path.Should().NotBeNullOrEmpty("couldn't find file {0} in directory or sub-directories from {1}", FileName, TestContext.CurrentContext.TestDirectory);
        return File.OpenRead(path);
    }

    private class LoggerSpy : ILogger
    {
        public List<FormattedLogEntry> Entries { get; } = new();

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new FormattedLogEntry(logLevel, formatter.Invoke(state, exception)));
        }

        public record FormattedLogEntry(LogLevel LogLevel, string Message);
    }
}
