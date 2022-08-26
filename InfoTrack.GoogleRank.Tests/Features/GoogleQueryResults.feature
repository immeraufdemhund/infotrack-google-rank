Feature: GoogleQueryResults

as a user I need to be able to see that I can get a set number of results from google

Background:
    Given the backend api has initialized

Scenario: Configured for 100 results
    Given the backend is configured to have 100 results returned
    When a api user calls GET on /api/GoogleRank
    Then a result with a data property has 100 values in it

Scenario: Results returned should contain summary
    When a api user calls GET on /api/GoogleRank
    Then a result with a summary property is a string value
    And the summary property contains 'www.infotrack.com'
    And the summary property has numbers in it
