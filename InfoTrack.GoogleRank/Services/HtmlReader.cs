using System.Text;

namespace InfoTrack.GoogleRank.Services;

public class HtmlReader
{
    private readonly ILogger _logger;
    public HtmlReader(ILogger<HtmlReader> logger)
    {
        _logger = logger;
    }

    public HtmlContainer ReadAll(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var sb = new StringBuilder();
        while (true)
        {
            var value = reader.Read();
            if (value < 1) break; // end of file
            if (Convert.ToChar(value) == '<')
            {
                var c = Convert.ToChar(reader.Read());
                if (c == '!')
                {
                    var _ = ParseComment(reader);
                }
                else
                {
                    var elementStart = $"<{c}{ReadTillEndElement(reader)}";
                    if(IsEmptyElement(elementStart))
                    {
                        _logger.LogDebug("Empty element contents {EmptyElement}", elementStart);
                    }
                    else
                    {
                        //TODO put in full elements
                    }
                }
            }
        }
        return new HtmlContainer(sb.ToString());
    }
    /// <summary>The empty elements in HTML are as follows:</summary>
    private static readonly string[] emptyElements = new string[]
    {
        "<area","<base","<br","<col","<embed","<hr","<img","<input",
        "<keygen","<link","<meta","<param","<source","<track","<wbr"
    };
    /// <summary>An empty element is an element from HTML, SVG, or MathML that cannot have any child nodes (i.e., nested elements or text nodes).
    /// In HTML, using a closing tag on an empty element is usually invalid. For example, <input type="text"></input> is invalid HTML.
    /// lets assume google got it right for now.</summary>
    private bool IsEmptyElement(string elementStart)
    {
        return emptyElements.Any(x => elementStart.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));
    }
    private string ParseComment(TextReader reader)
    {
        // reading a comment
        var sb = new StringBuilder("<!");
        while (true)
        {
            var a = Convert.ToChar(reader.Read());
            sb.Append(a);
            if (a == ' ')
            {
                if (sb.ToString().Equals("<!-- "))
                {
                    sb.Append(ReadTillEndOfComment(reader));
                    break;
                }
                if (sb.ToString().Equals("<!DOCTYPE "))
                {
                    sb.Append(ReadTillEndElement(reader));
                    break;
                }
                sb.Append(ReadTillEndElement(reader));
                break;
            }

        }
        return sb.ToString();
    }
    private string ReadTillEndOfComment(TextReader reader)
    {
        var sb = new StringBuilder();
        while (true)
        {
            var c = Convert.ToChar(reader.Read());
            sb.Append(c);
            // first level of stop comment
            if (c == '-')
            {
                c = Convert.ToChar(reader.Read());
                sb.Append(c);
                // second level of stop comment
                if (c == '-')
                {
                    c = Convert.ToChar(reader.Read());
                    sb.Append(c);
                    // final level of stop comment
                    if (c == '>')
                    {
                        break;
                    }
                }
            }
        }

        return sb.ToString();
    }
    private string ReadTillEndElement(TextReader reader)
    {
        var sb = new StringBuilder();
        while(true)
        {
            var c = Convert.ToChar(reader.Read());
            sb.Append(c);
            // end element marker
            if (c == '>')
            {
                break;
            }

        }
        return sb.ToString();
    }
}
public class HtmlCommand
{
    public string Content { get; internal set; }
}
public class HtmlElement
{
    public string Name { get; private set; }
    private readonly string _full;
    internal HtmlElement(string full)
    {
        _full = full;
    }
}
public class HtmlContainer
{
    public string Name { get; private set; }
    public HtmlElementCollection Children { get; } = new();

    private readonly string _full;
    internal HtmlContainer(string full)
    {
        _full = full;
    }
}
public class HtmlElementCollection
{
    public string Name { get; private set; }
    private List<HtmlElement> _elements;
}
