namespace InfoTrack.GoogleRank.Services;

public interface IHtmlScraper
{
    Task<object[]> SelectElementsWithCssSelector(Stream stream, ILogger logger);
}
