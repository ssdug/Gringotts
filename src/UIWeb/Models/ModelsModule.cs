using System.Linq;
using System.Reflection;
using Autofac;
using Wiz.Gringotts.UIWeb.Models.Features;
using Module = Autofac.Module;

namespace Wiz.Gringotts.UIWeb.Models
{
    public class ModelsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FeatureService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .As(type => type.GetInterfaces()
                    .Where(interfacetype => interfacetype.IsClosedTypeOf(typeof(ISearch<>))));

        }
    }
}