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
        await using var stream = OpenRead("efiling+integration.html");

        var result = await _scraper.SelectElementsWithCssSelector(stream, NullLogger.Instance);

        result.Should().HaveCount(99);
        result[0].Should().BeEquivalentTo(new { Index = 0, Url = "www.infotrack.com" });
        result[98].Should().BeEquivalentTo(new { Index = 98, Url = "www.lawpracticetoday.org" });
    }

    [Test]
    public async Task FoundVariation()
    {
        await using var stream = OpenRead("result174124.html");

        var result = await _scraper.SelectElementsWithCssSelector(stream, NullLogger.Instance);

        result.Should().HaveCount(99);
        result[0].Should().BeEquivalentTo(new { Index = 0, Url = "www.infotrack.com" });
        result[98].Should().BeEquivalentTo(new { Index = 98, Url = "support.taulia.com" });
    }

    private static Stream OpenRead(string fileName)
    {
        var path = Directory.EnumerateFiles(TestContext.CurrentContext.TestDirectory, fileName, SearchOption.AllDirectories).FirstOrDefault();
        path.Should().NotBeNullOrEmpty("couldn't find file {0} in directory or sub-directories from {1}", fileName, TestContext.CurrentContext.TestDirectory);
        return File.OpenRead(path);
    }
}
