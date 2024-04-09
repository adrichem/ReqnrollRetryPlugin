using Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin;
using Reqnroll.Bindings;
using Reqnroll.Bindings.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Adrichem.ReqnrollPlugIn.RetryStepPlugin.RetryBasedOnAttribute
{
    /// <summary>
    /// Retries failed steps having the <see cref="RetryAttribute"/>
    /// </summary>
    public class AttributeBasedRetryer : IRetryStepPluginConfiguration
    {
        public virtual bool RetryEnabledFor(BindingMatch match)
        {
            return GetStepDefMethodAttributes(match).Any(attrData => attrData.AttributeType == typeof(RetryAttribute));
        }
        
        protected virtual IEnumerable<CustomAttributeData> GetStepDefMethodAttributes(BindingMatch s)
        {
            if(s.StepBinding.Method is RuntimeBindingMethod)
            {
                return (s.StepBinding.Method as RuntimeBindingMethod).MethodInfo.CustomAttributes;
            }
            throw new InvalidOperationException($"{nameof(AttributeBasedRetryer)} is unable to retrieve the attributes on the step definition method.");
        }
    }

    /// <summary>
    /// Step definition should be retried when it fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryAttribute : Attribute { }
}
