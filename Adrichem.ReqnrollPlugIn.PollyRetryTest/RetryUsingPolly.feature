Feature: PollyAsRetryFramework
As author of step definitions, I want freedom to use my preferred  framework for handling retries.

Scenario: ProofOfConcept
	
	Given the amount of transient errors is 5
	When I set the max number of retries to 5
	Then these transient failures may not cause test failure
