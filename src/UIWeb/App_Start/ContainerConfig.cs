using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.Mvc;

namespace Wiz.Gringotts.UIWeb
{
    public class ContainerConfig
    {
        public static void RegisterContainer(Action<IContainer> onReady)
        {
            var builder = new ContainerBuilder();

            //Add application modules, one per assembly
            builder.RegisterAssemblyModules(typeof(MvcApplication).Assembly);

            //Add 3rd party integration modules
            builder.RegisterModule<NLogModule>();
            //needed for action filters, ensures ILogger is resolvable
            builder.RegisterModule<SimpleNLogModule>();

            var container = builder.Build();

            //Add 3rd party integrations that do not 
            //supply autofac modules
            container.ActivateGlimpse();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            onReady(container);
        }
    }
}