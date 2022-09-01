using System.Text.RegularExpressions;

namespace InfoTrack.GoogleRank.Services;

public class DirtyHtmlScraper : IHtmlScraper
{
    public async Task<IndexedUrl[]> SelectElementsWithCssSelector(Stream stream, ILogger logger)
    {
        using var reader = new StreamReader(stream);
        var text = await reader.ReadToEndAsync();
        var matches = ChunkHtmlIntoSpecificDivsFromHtmlString(text).ToArray();
        if (matches.Length > 0)
        {
            logger.LogDebug("found {Count} result divs", matches.Length);
        }
        else
        {
            logger.LogWarning("Did not find any search result divs to return results with");
            return Array.Empty<IndexedUrl>();
        }

        try
        {
            return matches
                .Select(resultElement => resultElement.Single(x => x.Value.Contains(GetSearchStringFromCssSelector(".BNeawe.UPmit.AP7Wnd"))))
                .Select(urlHeaderElement =>
                {
                    var endOfOpeningDivTagIndex = text.IndexOf(">", urlHeaderElement.Index);
                    var startOfClosingDivTag = text.IndexOf("</div>", endOfOpeningDivTagIndex);
                    var length = startOfClosingDivTag - endOfOpeningDivTagIndex;
                    return text.Substring(endOfOpeningDivTagIndex + 1, length - 1);
                })
                .Select(headerText => headerText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Trim())
                .Select((x, index) => new IndexedUrl(index, x))
                .ToArray();
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e, "my css search string for where the results are must be wrong");
            return Array.Empty<IndexedUrl>();
        }
    }

    private IEnumerable<List<Group>> ChunkHtmlIntoSpecificDivsFromHtmlString(string html)
    {
        var all = Regex.Matches(html, @"<(.*?)>", RegexOptions.Multiline);
        var selector = GetSearchStringFromCssSelector(".Gx5Zad.fP1Qef.xpd.EtOod.pkphOe");

        var enumerator = all.Select(a => a.Groups[0]).GetEnumerator();
        while (enumerator.MoveNext())
        {
            var currentGroupToCheck = enumerator.Current;
            if (!currentGroupToCheck.Value.Contains(selector)) continue;

            yield return GetCurrentElementAndAllChildren(enumerator);
        }
    }

    private static List<Group> GetCurrentElementAndAllChildren(IEnumerator<Group> enumerator)
    {
        var newListToReturn = new List<Group>();
        newListToReturn.Add(enumerator.Current);
        var depth = 1;
        while (depth != 0)
        {
            enumerator.MoveNext();
            newListToReturn.Add(enumerator.Current);

            var value = enumerator.Current.Value;
            var isClosingTag = value.StartsWith("</");
            if (isClosingTag)
            {
                depth--;
                if (depth == 0)
                {
                    break;
                }
            }
            else
            {
                // skip empty tags
                if (IsEmptyTag(value)) continue;

                depth++;
            }
        }

        return newListToReturn;
    }

    private static bool IsEmptyTag(string element)
    {
        var match = Regex.Match(element, @"<(?'tagname'\w+).*>");
        if (!match.Success) return false;

        var tagName = match.Groups["tagname"].Value;
        return emptyTags.Contains(tagName);
    }

    // https://developer.mozilla.org/en-US/docs/Glossary/Empty_element
    private static readonly string[] emptyTags = new string[]
    {
        "area", "br", "col", "embed", "hr", "img",
        "input", "keygen", "link", "meta", "param", "source", "track", "wbr"
    };

    private static string GetSearchStringFromCssSelector(string cssSelector)
    {
        if (cssSelector.Contains(" "))
            throw new Exception("can't handle descendants yet");

        var idMatch = Regex.Match(cssSelector, @"#(?'id'\w+)");
        if (idMatch.Success)
        {
            return $"id=\"{idMatch.Groups["id"].Value}\"";
        }

        var classMatches = Regex.Matches(cssSelector, @".(?'className'\w+)");
        var classNames = string.Join(" ", classMatches.Select(m => m.Groups["className"].Value));
        return $"class=\"{classNames}\"";
    }
}
