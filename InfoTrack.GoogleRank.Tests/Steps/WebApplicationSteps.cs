using FluentAssertions;
using InfoTrack.Google.Tests.Drivers;
using InfoTrack.GoogleRank;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow.Assist;

namespace InfoTrack.Google.Tests.Steps;

[Binding]
public sealed class WebApplicationSteps
{
    private readonly WebApplicationDriver _webApplicationDriver;

    public WebApplicationSteps(WebApplicationDriver webApplicationDriver)
    {
        _webApplicationDriver = webApplicationDriver;
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
}
