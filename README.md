# A Reqnroll Proof Of Concept to retry failed steps

## Why retry?
The System Under Test (SUT) runs asynchronously and needs time to reach the correct state. During this time the various `Then` steps may not fail the scenario. 

In essence execution of a single  `Then` step needs to:
1. Complete when it runs successfully. This avoids unecessary delays.
2. Retry when it throws during the allowable time.
3. Stop and throw last exception when the maximum time is reached.

## How the step definition author provides logic and decisions for retry 
The author registers a `IRetryStepPluginConfiguration` in the scenario container. This object has 1 method to decide if a `BindingMatch` should be retried or not.

They also register a `IAsyncRetryer` having the `Task RetryAsync(Action)` method. This object is responsible for doing the retry logic.

For example:
```csharp
//This test project wants all `Then` steps to be retried.
public class RetryThen : IRetryStepPluginConfiguration
{
    public bool RetryEnabledFor(BindingMatch match) => match.StepBinding.StepDefinitionType == StepDefinitionType.Then;
}

[BeforeScenario]
public static void BeforeScenario(IObjectContainer c)
{
    c.RegisterInstanceAs<IRetryStepPluginConfiguration>(new RetryThen());
    c.RegisterInstanceAs<IAsyncRetryer>(new PollyRetryer());
}

[Then("this step definition will be retried in case of transient failures")]
public void DoCheck() => MySystem.State.Should().BeOK();
```

## How the plugin is implemented 
The plugin registers a custom `ITestExecutionEngine`. This custom engine derives from the default ReqnRoll `TestExecutionEngine` but overrides the `ExecuteStepMatchAsync` method to: 
1. Retrieve the `IRetryStepPluginConfiguration` from the ScenarioContainer and ask if the current `BindingMatch` should be retried.
2. If retry is needed, it retrieves an `IAsyncRetryer` from the ScenarioContainer to perform the retry logic on the invocation of the binding like this
```csharp
await Retryer.RetryAsync(async () => await _bindingInvoker.InvokeBindingAsync(match.StepBinding, _contextManager, arguments, _testTracer, durationHolder));
```

