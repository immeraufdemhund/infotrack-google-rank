using System.Text;
using System.Text.RegularExpressions;

namespace InfoTrack.GoogleRank.Services;

public class DirtyHtmlScraper : IHtmlScraper
{
    private delegate IndexedUrl[] CssSelector(TextReader reader, ILogger logger);
    public Task<IndexedUrl[]> SelectElementsWithCssSelector(Stream stream, ILogger logger)
    {
        using var reader = new StreamReader(stream);

        SkipUntilDivId_search(reader);
        var depth = 0;
        while (true)
        {
            var c = GuardAgainstEndOfStream(reader);
            if (c == '<')
            {
                var element = ReadToEndOfElement(reader, "<");
                if (element.StartsWith("<div"))
                {
                    depth++;
                }

                if (element.StartsWith("</div>"))
                {
                    depth--;
                }
            }
        }
    }

    private static string GetId(string element) => Regex.Match(element, @" id=[""'](?'id'.+?)[""']").Groups["id"].Value;
    private static void SkipUntilDivId_search(TextReader s)
    {
        while (true)
        {
            var c = GuardAgainstEndOfStream(s);
            if (c == '<')
            {
                var element = ReadToEndOfElement(s, "<");
                if (element.StartsWith("<div"))
                {
                    if (GetId(element).Equals("search"))
                    {
                        break;
                    }
                }
            }
        }
    }

    private static string ReadToEndOfElement(TextReader s, string elementStart)
    {
        var sb = new StringBuilder(elementStart);
        while (true)
        {
            var c = GuardAgainstEndOfStream(s);
            sb.Append(c);
            if (c == '>')
            {
                return sb.ToString();
            }
        }
    }

    private static char GuardAgainstEndOfStream(TextReader s)
    {
        var value = s.Read();
        if (value < 1) throw new Exception("Unexpected End Of Stream condition");
        return Convert.ToChar(value);
    }
}
