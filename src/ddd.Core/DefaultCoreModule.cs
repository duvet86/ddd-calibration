using Autofac;
using ddd.Core.Interfaces;
using ddd.Core.Services;

namespace ddd.Core
{
    public class DefaultCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ToDoItemSearchService>()
                .As<IToDoItemSearchService>().InstancePerLifetimeScope();
        }
    }
}