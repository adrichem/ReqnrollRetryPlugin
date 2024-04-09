Feature: RetryBasedOnAttribute

Scenario: BeforeStepHookBehavior
The [BeforeStep] hook may only be called once, even when steps are retried.

	Given I set the next flaky action to fail 3x before passing
	And I reset the counter for the BeforeStep hook
	
	When I do a flaky action marked with the attribute 
	Then the number of BeforeStep hook invocations must 2

Scenario: AfterStepHookBehavior
The [BeforeStep] hook may only be called once, even when steps are retried.

	Given I set the next flaky action to fail 3x before passing
	And I reset the counter for the AfterStep hook
	
	When I do a flaky action marked with the attribute 
	Then the number of AfterStep hook invocations must 2

Scenario: RetryStepWithAttribute
A step definition marked with the [Retry] attribute must be retried when it fails.

	Given I set the next flaky action to fail 3x before passing
	When I do a flaky action marked with the attribute 
	Then the number of retries must be 3

Scenario: RetryAsyncStepWithAttribute
An async step definition marked with the [Retry] attribute must be retried when it fails.

	Given I set the next flaky action to fail 3x before passing
	When I do an async flaky action marked with the attribute 
	Then the number of retries must be 3