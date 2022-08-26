namespace InfoTrack.GoogleRank.Services;

public class DirtyHtmlScraper
{
    private readonly string _html;
    public static DirtyHtmlScraper Parse(string html) => new DirtyHtmlScraper(html);
    private DirtyHtmlScraper(string html)
    {
        _html = html;
    }

    public object[] GetAllElements(string cssSelector)
    {
        return new object[100];
    }
}
