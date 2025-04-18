using System;
using Reqnroll.BoDi;
using Reqnroll.Analytics;
using Reqnroll.ErrorHandling;
using Reqnroll.Events;
using Reqnroll.Plugins;
using Reqnroll.Tracing;
using Reqnroll.UnitTestProvider;
using Reqnroll.Infrastructure;
using Reqnroll.Bindings;
using System.Threading.Tasks;
using Reqnroll.Configuration;
using Reqnroll;
using System.Diagnostics;

namespace Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin
{
    public class RetryStepTestExecutionEngine : TestExecutionEngine
    {
        readonly IAsyncBindingInvoker _bindingInvoker;
        readonly ITestThreadExecutionEventPublisher _testThreadExecutionEventPublisher;
        readonly ITestTracer _testTracer;
        readonly IContextManager _contextManager;

        public RetryStepTestExecutionEngine(
            IStepFormatter stepFormatter,
            ITestTracer testTracer,
            IErrorProvider errorProvider,
            IStepArgumentTypeConverter stepArgumentTypeConverter,
            ReqnrollConfiguration reqnrollConfiguration,
            IBindingRegistry bindingRegistry,
            IUnitTestRuntimeProvider unitTestRuntimeProvider,
            IContextManager contextManager,
            IStepDefinitionMatchService stepDefinitionMatchService,
            IAsyncBindingInvoker bindingInvoker,
            IObsoleteStepHandler obsoleteStepHandler,
            IAnalyticsEventProvider analyticsEventProvider,
            IAnalyticsTransmitter analyticsTransmitter,
            ITestRunnerManager testRunnerManager,
            IRuntimePluginTestExecutionLifecycleEventEmitter runtimePluginTestExecutionLifecycleEventEmitter,
            ITestThreadExecutionEventPublisher testThreadExecutionEventPublisher,
            ITestPendingMessageFactory testPendingMessageFactory,
            ITestUndefinedMessageFactory testUndefinedMessageFactory,
            ITestObjectResolver testObjectResolver,
            ITestRunContext testRunContext)
        : base(stepFormatter, testTracer, errorProvider, stepArgumentTypeConverter, reqnrollConfiguration
              , bindingRegistry, unitTestRuntimeProvider, contextManager, stepDefinitionMatchService, bindingInvoker
              , obsoleteStepHandler, analyticsEventProvider, analyticsTransmitter, testRunnerManager
              , runtimePluginTestExecutionLifecycleEventEmitter, testThreadExecutionEventPublisher
              , testPendingMessageFactory, testUndefinedMessageFactory, testObjectResolver, testRunContext)
        {
            _bindingInvoker = bindingInvoker;
            _testThreadExecutionEventPublisher = testThreadExecutionEventPublisher;
            _testTracer = testTracer;
            _contextManager = contextManager;
        }

        public Guid Instance { get; } = Guid.NewGuid();
        protected override async Task ExecuteStepMatchAsync(BindingMatch match, object[] arguments, DurationHolder durationHolder)
        {

            _testThreadExecutionEventPublisher.PublishEvent(new StepBindingStartedEvent(match.StepBinding));

            durationHolder.Duration = default;
            Stopwatch stopwatch = new Stopwatch();

            try
            {

                IRetryStepPluginConfiguration cfg = ScenarioContext.ScenarioContainer.Resolve<IRetryStepPluginConfiguration>();
                if (cfg.RetryEnabledFor(match))
                {
                    var Retryer = ScenarioContext.ScenarioContainer.Resolve<IAsyncRetryer>();
                    await Retryer.RetryAsync(async () => await _bindingInvoker.InvokeBindingAsync(match.StepBinding, _contextManager, arguments, _testTracer, durationHolder));
                }
                else
                {
                    await _bindingInvoker.InvokeBindingAsync(match.StepBinding, _contextManager, arguments, _testTracer, durationHolder);
                }

            }
            finally
            {
                stopwatch.Stop();
                durationHolder.Duration = stopwatch.Elapsed;
                _testThreadExecutionEventPublisher.PublishEvent(new StepBindingFinishedEvent(match.StepBinding, durationHolder.Duration));
            }
        }
    }
}
