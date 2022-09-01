@AcceptanceTests @IntegrationTests
Feature: GoogleQueryResults

as a user I need to be able to see that I can get a set number of results from google

Background:
    Given the backend api has initialized

Scenario: Configured for 100 results
    Given the backend is configured to have 100 results returned
    When a api user calls GET on /GoogleRank
    Then a json result with a data property has 99 values in it
    And the summary property contains 'www.infotrack.com'
    And the summary property has numbers in it
    And the first result in data should have a non empty string value in the url property
    And a numeric value '0' in index property
