using Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin;
using Polly;
using Polly.Retry;

namespace Adrichem.ReqnrollPlugIn.PollyRetry
{
    public class PollyRetryer : IAsyncRetryer
    {
        public int nRetries { get; set; } = 0;
        public RetryStrategyOptions Options { get; set; } = new RetryStrategyOptions();

        public PollyRetryer() => Options.OnRetry = async (a) => await Task.Run(() => nRetries++);

        public async Task RetryAsync(Func<Task> action)
        {
            nRetries = 0;
            await new ResiliencePipelineBuilder().AddRetry(Options).Build().ExecuteAsync(async _ => { await action(); });
        }
    }
}