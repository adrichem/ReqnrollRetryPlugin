using Adrichem.ReqnrollPlugIn.PollyRetry;
using Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin;
using Reqnroll.BoDi;
using Reqnroll;
using Reqnroll.Bindings;
using System;


namespace Adrichem.ReqnrollPlugIn.PollyRetryerTest
{
    public class RetryThen : IRetryStepPluginConfiguration
    {
        public virtual bool RetryEnabledFor(BindingMatch match) => match.StepBinding.StepDefinitionType == StepDefinitionType.Then;
    }

    [Binding]
    public class PollyDemoStepDefinitions
    {
        readonly PollyRetryer Retryer;
        int nTimesToFail;

        [BeforeScenario]
        public static void BeforeScenario(IObjectContainer c)
        {
            c.RegisterInstanceAs<IRetryStepPluginConfiguration>(new RetryThen());
            c.RegisterInstanceAs<IAsyncRetryer>(new PollyRetryer());
        }

        public PollyDemoStepDefinitions(PollyRetryer P)
        {
            Retryer = P;
            Retryer.Options.MaxDelay = TimeSpan.Zero;
        }

        [Given(@"the amount of transient errors is {int}")]
        public void SetNumberOfFails(int n) => nTimesToFail = n;

        [When(@"I set the max number of retries to {int}")]
        public void SetRetryLimit(int n) => Retryer.Options.MaxRetryAttempts = n;

        [Then(@"these transient failures may not cause test failure")]
        public void SimulateFlakyness()
        {
            if (nTimesToFail >= 1)
            {
                nTimesToFail--;
                Console.WriteLine($"Flaky action is throwing.");
                throw new InvalidOperationException();
            }
            Console.WriteLine("Flaky action is passing.");
        }
    }
}
