@AcceptanceTests @IntegrationTests
Feature: GoogleQueryResults

as a user I need to be able to see that I can get a set number of results from google

Background:
    Given the backend api has initialized

Scenario: Configured for 100 results
    Given the backend is configured to have 100 results returned
    When a api user calls GET on /GoogleRank
    Then a json result with a data property has 100 values in it

Scenario: Results returned should contain summary
    When a api user calls GET on /GoogleRank
    Then a json result with a summary property is a string value
    And the summary property contains 'www.infotrack.com'
    And the summary property has numbers in it

Scenario: Results returned in data have details
    Given the backend is configured to have 1 results returned
    When a api user calls GET on /GoogleRank
    Then the first result in data should have a url in linkedTo property
    And a boolean value in isCompanyUrl property
    And a numeric value '1' in position property
