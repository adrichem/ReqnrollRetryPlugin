using Reqnroll.Bindings;

namespace Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin
{
    /// <summary>
    /// Decides which step-definitions need retry logic of <see cref="IAsyncRetryer"/>
    /// </summary>
    public interface IRetryStepPluginConfiguration
    {
        bool RetryEnabledFor(BindingMatch b);
    }
}
