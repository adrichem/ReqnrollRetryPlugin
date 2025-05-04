# A Reqnroll plugin to automatically retry failed steps

## Why retry?
When your System Under Test (SUT) runs asynchronously, it needs time to reach the correct state. During this time your `Then` steps should not fail, execution of it needs to:
1. Complete when it runs successfully. This avoids unnecessary delays.
2. Retry when it throws during the allowable time.
3. Stop and throw last exception when the maximum time is reached.

## How to use it
Register a `IRetryStepPluginConfiguration` in the scenario container. This object is responsible to decide which step-definitions are eligible for retry. Additionally, register a `IAsyncRetryer`. This object is responsible for doing the retry logic.

For example:
```csharp
[BeforeScenario]
public static void BeforeScenario(IObjectContainer c)
{
    c.RegisterInstanceAs<IRetryStepPluginConfiguration>(new RetryThen());
    c.RegisterInstanceAs<IAsyncRetryer>(new PollyRetryer());
}

//This test project wants all `Then` steps to be retried.
public class RetryThen : IRetryStepPluginConfiguration
{
    public bool RetryEnabledFor(BindingMatch match) 
    => match.StepBinding.StepDefinitionType == StepDefinitionType.Then;
}

[Then("this step definition will be retried in case of transient failures")]
public void DoCheck() => MySystem.State.Should().BeOK();
```

## How the plugin is implemented 
The plugin registers a custom `ITestExecutionEngine`. This custom engine derives from the default Reqnroll `TestExecutionEngine` but overrides the `ExecuteStepMatchAsync` method to: 
1. Retrieve the `IRetryStepPluginConfiguration` from the ScenarioContainer and ask if the current `BindingMatch` should be retried.
2. If retry is needed, it retrieves an `IAsyncRetryer` from the ScenarioContainer to perform the retry logic on the invocation of the binding like this
```csharp
await Retryer.RetryAsync(async () => await _bindingInvoker.InvokeBindingAsync(match.StepBinding, _contextManager, arguments, _testTracer, durationHolder));
```