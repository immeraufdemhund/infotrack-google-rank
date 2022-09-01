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
        _scraper = new DirtyHtmlScraper();
    }

    [Test]
    public async Task ReturnsStringArrayWithSpecificClass()
    {
        await using var stream = ReadTop100EfilingIntegrationSearchPage();

        var result = await _scraper.SelectElementsWithCssSelector(stream, NullLogger.Instance);

        result.Should().HaveCount(99);
        result[0].Should().BeEquivalentTo(new { Index = 0, Url = "www.infotrack.com" });
        result[98].Should().BeEquivalentTo(new { Index = 98, Url = "www.lawpracticetoday.org" });
    }

    private Stream ReadTop100EfilingIntegrationSearchPage()
    {
        const string FileName = "efiling+integration.html";
        var path = Directory.EnumerateFiles(TestContext.CurrentContext.TestDirectory, FileName, SearchOption.AllDirectories).FirstOrDefault();
        path.Should().NotBeNullOrEmpty("couldn't find file {0} in directory or sub-directories from {1}", FileName, TestContext.CurrentContext.TestDirectory);
        return File.OpenRead(path);
    }
}
