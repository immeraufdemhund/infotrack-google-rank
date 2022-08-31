namespace InfoTrack.GoogleRank.Services;

public interface IHtmlScraper
{
    Task<IndexedUrl[]> SelectElementsWithCssSelector(Stream stream, ILogger logger);
}

public class IndexedUrl
{
    public int Index { get; }
    public string Url { get; }
    public IndexedUrl(int index, string url)
    {
        Index = index;
        Url = url;
    }
}
