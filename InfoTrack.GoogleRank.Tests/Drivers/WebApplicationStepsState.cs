using System.Text.Json;

namespace InfoTrack.Google.Tests.Drivers;

public class WebApplicationStepsState
{
    public HttpResponseMessage CurrentResponseFromApi { get; set; }
    public JsonDocument CurrentContentFromCurrentResponse { get; set; }
}
