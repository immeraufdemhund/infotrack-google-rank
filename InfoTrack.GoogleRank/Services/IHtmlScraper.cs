namespace InfoTrack.GoogleRank.Services;

public interface IHtmlScraper
{
    Task<IndexedUrl[]> SelectElementsWithCssSelector(Stream stream, ILogger logger);
}

public readonly struct IndexedUrl
{
    public readonly int Index;
    public readonly string Url;
    public IndexedUrl(int index, string url)
    {
        Index = index;
        Url = url;
    }
}
