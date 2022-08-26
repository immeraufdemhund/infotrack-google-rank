using InfoTrack.GoogleRank.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfoTrack.GoogleRank.Controllers;

[ApiController]
[Route("[controller]")]
public class GoogleRankController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly GoogleRankService _service;

    public GoogleRankController(ILogger<GoogleRankController> logger, GoogleRankService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("getting rank from google for efiling+integration");
        var rawHtml = await _service.SearchGoogleFor("efiling+integration");
        var scraper = DirtyHtmlScraper.Parse(rawHtml);
        var data = scraper.GetAllElements(".element");
        return new OkObjectResult(new {data});
    }
}
