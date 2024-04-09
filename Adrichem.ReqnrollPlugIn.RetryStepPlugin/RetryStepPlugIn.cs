using Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin;
using Reqnroll.Infrastructure;
using Reqnroll.Plugins;
using Reqnroll.UnitTestProvider;

[assembly: RuntimePlugin(typeof(RetryStepPlugin))]
namespace Adrichem.ReqnrollPlugIn.RetryStep.ReqnrollPlugin
{
    public class RetryStepPlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeTestThreadDependencies += RuntimePluginEvents_CustomizeTestThreadDependencies;
        }

        void RuntimePluginEvents_CustomizeTestThreadDependencies(object sender, CustomizeTestThreadDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<ITestExecutionEngine>(typeof(RetryStepTestExecutionEngine));
        }
    }
}
