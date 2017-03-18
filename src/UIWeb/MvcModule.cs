using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Wiz.Gringotts.UIWeb.Filters;

namespace Wiz.Gringotts.UIWeb
{
    public class MvcModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // STANDARD MVC SETUP:
            var assembly = typeof(MvcApplication).Assembly;

            builder.RegisterControllers(assembly)
                .PropertiesAutowired();
            builder.RegisterModelBinders(assembly)
                .PropertiesAutowired();
            builder.RegisterFilterProvider();
            builder.RegisterModelBinderProvider();
            builder.RegisterModule<AutofacWebTypesModule>();

            //global filters
            builder.RegisterType<HandleErrorAttribute>()
                .AsExceptionFilterFor<Controller>();

            builder.RegisterType<Transaction>()
                .AsActionFilterFor<Controller>();

            builder.RegisterType<NonTenant>()
                .AsActionFilterFor<Controller>();

            builder.RegisterType<Features>()
                .AsActionFilterFor<Controller>();

            builder.RegisterType<ReturnUrl>()
                .AsActionFilterFor<Controller>();

            builder.RegisterType<Negotiate>()
                .AsActionFilterFor<Controller>();

            builder.RegisterType<EnvironmentMonitor>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}