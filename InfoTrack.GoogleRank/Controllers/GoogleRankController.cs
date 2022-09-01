using InfoTrack.GoogleRank.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfoTrack.GoogleRank.Controllers;

[ApiController]
[Route("[controller]")]
public class GoogleRankController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly GoogleRankService _service;
    private readonly IHtmlScraper _scraper;

    public GoogleRankController(ILogger<GoogleRankController> logger, GoogleRankService service, IHtmlScraper scraper)
    {
        _logger = logger;
        _service = service;
        _scraper = scraper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("getting rank from google for efiling+integration");
        var rawHtml = await _service.SearchGoogleFor("efiling+integration");
        var data = await _scraper.SelectElementsWithCssSelector(rawHtml, _logger);
        const string searchString = "www.infotrack.com";
        var indexOfInfotrack = data.FirstOrDefault(x => x.Url.Contains(searchString));
        var summary = indexOfInfotrack == null ?
            $"Didn't find {searchString} in the search results" :
            $"Found {searchString} with a rank of {indexOfInfotrack.Index + 1}";
        return new OkObjectResult(new {data, summary});
    }
}
