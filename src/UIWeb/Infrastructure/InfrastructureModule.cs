using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Wiz.Gringotts.UIWeb.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                .Where(type => type.Namespace.Contains(this.GetType().Namespace))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .AsSelf();
        }
    }
}