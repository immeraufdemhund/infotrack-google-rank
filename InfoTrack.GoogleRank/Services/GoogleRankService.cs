using System.Web;
using Microsoft.Extensions.Options;

namespace InfoTrack.GoogleRank.Services;

public class GoogleRankService
{
    private readonly HttpClient _client;
    private readonly IOptionsMonitor<GoogleSettings> _monitor;
    private readonly ILogger _logger;

    public GoogleRankService(HttpClient client, IOptionsMonitor<GoogleSettings> monitor, ILogger<GoogleRankService> logger)
    {
        _client = client;
        _monitor = monitor;
        _logger = logger;
    }
    public async Task<Stream> SearchGoogleFor(string query)
    {
        var limit = _monitor.CurrentValue.QueryResultLimit;
        var request = new HttpRequestMessage(HttpMethod.Get, $"search?num={limit}&q={HttpUtility.UrlEncode(query)}");
        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError("Problem with talking to google {Status}, {Reasons}: {Content}", response.StatusCode, response.ReasonPhrase, content);
            throw new Exception(content);
        }

        return await response.Content.ReadAsStreamAsync();
    }
}
