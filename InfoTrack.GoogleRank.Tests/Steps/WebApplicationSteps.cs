using System.Net;
using System.Text.Json;
using FluentAssertions;
using InfoTrack.Google.Tests.Drivers;
using InfoTrack.GoogleRank;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow.Assist;

namespace InfoTrack.Google.Tests.Steps;

[Binding]
public sealed class WebApplicationSteps
{
    private readonly WebApplicationStepsState _state;
    private readonly WebApplicationDriver _webApplicationDriver;

    public WebApplicationSteps(WebApplicationDriver webApplicationDriver, WebApplicationStepsState state)
    {
        _webApplicationDriver = webApplicationDriver;
        _state = state;
    }

    [Given("the backend api has initialized")]
    public void InitializeWebApplication()
    {
        _webApplicationDriver.Initialize();
    }

    [Given(@"the backend is configured to have (\d+) results returned")]
    public void SetResultsReturnedConfigurationValue(int number)
    {
        _webApplicationDriver.SetConfigValue("QueryResultLimit", number);
    }

    [When("I get the IConfiguration service")]
    public void GuardAgainstNullConfigurationService()
    {
        _webApplicationDriver.Get<IConfiguration>().Should().NotBeNull();
    }

    [Then("I can get values for the following configuration keys")]
    public void CheckConfigurationForValues(Table table)
    {
        var section = _webApplicationDriver.Get<IConfiguration>();
        foreach (var keyNamesRow in table.Rows)
        {
            string key = keyNamesRow[0];
            section[key].Should()
                .NotBeNullOrEmpty("Missing config value: " +
                                  "\"{0}\": \"value\"" +
                                  " in the appsettings.Development or appsettings.json" +
                                  " in InfoTrack.GoogleRank project", key);
        }
    }

    [When(@"I change the value of (.*) to (.*)")]
    public void ChangeConfigValue(string key, string value)
    {
        _webApplicationDriver.SetConfigValue(key, value);
    }

    [When(@"a api user calls GET on (.*)")]
    public async Task MakeCallToApi(string relativeUri)
    {
        using var client = _webApplicationDriver.Client;
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
        _state.CurrentResponseFromApi = await client.SendAsync(httpRequest);
    }

    [Then(@"the default value for (.*) should be (.*)")]
    [Then(@"when I get (.*) from IConfiguration it should be (.*)")]
    public void CheckDefaultValueForKey(string key, string value)
    {
        _webApplicationDriver.GetConfigValue(key).Should().Be(value, "because the default value has been defined as a test");
    }

    [Then("the current snapshot of GoogleSettings should be")]
    public void CheckSettings(Table table)
    {
        var currentSettings = _webApplicationDriver.Get<IOptionsMonitor<GoogleSettings>>().CurrentValue;
        table.CompareToInstance(currentSettings);
    }

    [Then("a json result with a data property has 100 values in it")]
    public async Task CheckContent()
    {
        var response = _state.CurrentResponseFromApi;
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        var content = await response.Content.ReadAsStreamAsync();
        var jsonDocument = await JsonDocument.ParseAsync(content);
        jsonDocument.RootElement.TryGetProperty("data", out var dataElement)
            .Should()
            .BeTrue("because I expect the json object returned to have a data array");
        dataElement.GetArrayLength().Should().Be(100, "I'm expecting 100 values returned from google");
    }
}
