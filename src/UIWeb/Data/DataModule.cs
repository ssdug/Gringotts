using Autofac;
using Wiz.Gringotts.UIWeb.Data.Interceptors;
using Wiz.Gringotts.UIWeb.Models;

namespace Wiz.Gringotts.UIWeb.Data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InterceptingApplicationDbContext>()
                .AsImplementedInterfaces()
                .As<ApplicationDbContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof (Lookup<>))
                .As(typeof (ILookup<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<AuditChangeInterceptor>()
                .As<IInterceptor>();

            builder.RegisterType<TenantOrganizationInterceptor>()
                .As<IInterceptor>();
        }
    }
}