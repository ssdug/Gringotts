using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;

namespace Wiz.Gringotts.UIWeb
{
    public class MvcApplication : HttpApplication
    {
        private IContainer container;
        private IEnvironmentMonitor monitor { get; set; }

        protected void Application_Start()
        {
            ContainerConfig.RegisterContainer(c =>
            {
                this.container = c;
            });
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            monitor = container.Resolve<IEnvironmentMonitor>();
            monitor.AppStarted();

        }

        protected void Application_End()
        {
            monitor.AppEnded();
        }

    }
}
