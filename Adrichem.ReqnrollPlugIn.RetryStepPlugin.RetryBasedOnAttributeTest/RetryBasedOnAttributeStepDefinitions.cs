using Adrichem.ReqnrollPlugIn.PollyRetry;
using Adrichem.ReqnrollPlugIn.RetryStepPlugin.RetryBasedOnAttribute;
using BoDi;
using FluentAssertions;
using System;
using Reqnroll;
using Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin;
using System.Threading.Tasks;

namespace Adrichem.ReqnrollPlugIn.RetryStepPlugin.RetryBasedOnAttributeTest
{
    [Binding]
    public class RetryBasedOnAttributeStepDefinitions
    {
        readonly PollyRetryer Retryer;
        int nTimesToFail;
        int nBeforeSteps;
        int nAfterSteps;

        [BeforeScenario]
        public static void BeforeScenario(IObjectContainer c)
        {
            c.RegisterInstanceAs<IAsyncRetryer>(new PollyRetryer());
            c.RegisterInstanceAs<IRetryStepPluginConfiguration>(new AttributeBasedRetryer());
        }

        [BeforeStep] public void BeforeStep() => nBeforeSteps++;

        [AfterStep] public void AfterStep() => nAfterSteps++;

        public RetryBasedOnAttributeStepDefinitions(PollyRetryer R)
        {
            Retryer = R;
            Retryer.Options.MaxDelay = TimeSpan.FromMilliseconds(1);
        }

        [Given(@"I set the max number of retries to ([+-]{0,1}[1-9][0-9]*)")]
        public void SetRetryLimit(int n) => Retryer.Options.MaxRetryAttempts = n;

        [Given(@"I set the next flaky action to fail (\d+)x before passing")]
        public void SetNumberOfFails(int n) => nTimesToFail = n;

        [Given(@"I reset the counter for the BeforeStep hook")]
        public void ResetBeforeStepCounter() => nBeforeSteps = 0;

        [Given(@"I reset the counter for the AfterStep hook")]
        public void ResetAfterStepCounter() => nAfterSteps = 0;

        [Retry, When(@"I do a flaky action marked with the attribute")]
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

        [Retry, When(@"I do an async flaky action marked with the attribute")]
        public async Task SimulateFlakynessAsync() => await Task.Run(SimulateFlakyness);

        [Then(@"the number of retries must be (\d+)")]
        public void NumberOfInvocationsMustBe(int Expected) => Retryer.nRetries.Should().Be(Expected);

        [Then(@"the number of BeforeStep hook invocations must (\d+)")]
        public void BeforeStepCountMustBe(int Expected) => nBeforeSteps.Should().Be(Expected);

        [Then(@"the number of AfterStep hook invocations must (\d+)")]
        public void AfterStepCountMustBe(int Expected) => nAfterSteps.Should().Be(Expected);

    }
}