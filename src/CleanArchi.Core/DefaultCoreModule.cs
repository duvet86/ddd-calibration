using Autofac;
using CleanArchi.Core.Interfaces;
using CleanArchi.Core.Services;

namespace CleanArchi.Core;

public class DefaultCoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ToDoItemSearchService>()
            .As<IToDoItemSearchService>().InstancePerLifetimeScope();
    }
}
