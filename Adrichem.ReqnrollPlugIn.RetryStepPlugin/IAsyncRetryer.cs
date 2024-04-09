using System;
using System.Threading.Tasks;

namespace Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin
{
    /// <summary>
    /// Implements retry logic.
    /// </summary>
    public interface IAsyncRetryer
    {
        /// <summary>
        ///  Retries <paramref name="action"/> as long as needed.
        /// </summary>
        Task RetryAsync(Func<Task> action);
    }
}
