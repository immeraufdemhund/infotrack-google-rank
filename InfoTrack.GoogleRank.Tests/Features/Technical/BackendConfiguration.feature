Feature: BackendConfiguration

I need to know that configuration values are added and used properly

Scenario: When project starts I can access configuration values directly
    Given the backend api has initialized
    When I get the IConfiguration service
    Then I can get values for the following configuration keys
    | Key              |
    | QueryResultLimit |

Scenario: I need to be able to change query result limit in code for testing
    Given the backend api has initialized
    Then the default value for QueryResultLimit should be 100
    When I change the value of QueryResultLimit to 50
    Then when I get QueryResultLimit from IConfiguration it should be 50
