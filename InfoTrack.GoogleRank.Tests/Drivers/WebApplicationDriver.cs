using InfoTrack.GoogleRank;
using Microsoft.AspNetCore.Mvc.Testing;

namespace InfoTrack.Google.Tests.Drivers;

public sealed class WebApplicationDriver : IDisposable
{
    internal T Get<T>() => _application.Services.GetRequiredService<T>();

    private WebApplicationFactory<Program> _application;

    public void Initialize()
    {
        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(ConfigureTestServicesToReplaceRealServices);
    }

    private void ConfigureTestServicesToReplaceRealServices(IWebHostBuilder builder)
    {
        builder
            .ConfigureServices(collection =>
            {
                collection.AddOptions<GoogleSettings>()
                    .Configure<IConfiguration>((settings, configuration) => configuration.Bind(settings));
            })
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddSimpleConsole();
                loggingBuilder.AddFilter("*", LogLevel.Trace);
            });
    }

    public void SetConfigValue(string key, object value)
    {
        Get<IConfiguration>()[key] = value.ToString();
    }

    public string GetConfigValue(string key) => Get<IConfiguration>()[key];

    public void Dispose()
    {
        _application.Dispose();
    }
}
