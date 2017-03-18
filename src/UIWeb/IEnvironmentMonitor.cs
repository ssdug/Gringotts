using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Autofac.Extras.NLog;

namespace Wiz.Gringotts.UIWeb
{
    public interface IEnvironmentMonitor
    {
        void AppStarted();
        void AppEnded();
    }

    public class EnvironmentMonitor : IEnvironmentMonitor
    {
        public ILogger Logger { get; set; }

        public void AppStarted()
        {
            Logger.Info("Application Started");
        }

        public void AppEnded()
        {
            try
            {
                Logger.Error("HostingEnvironment.ShutdownReason::" + HostingEnvironment.ShutdownReason);
                var runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime",
                       BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
                if (runtime == null) return;

                var shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

                var shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

                Logger.Error("HttpRuntime shutdown reason: {0} \n{1}", shutDownMessage, shutDownStack);

            }
            catch (Exception ex)
            {
                Logger.Error("Failed to log HttpRuntime shutdown reason", ex);
            }
        }
    }
}