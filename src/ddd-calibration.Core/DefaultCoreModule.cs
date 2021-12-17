using Autofac;
using ddd_calibration.Core.Interfaces;
using ddd_calibration.Core.Services;

namespace ddd_calibration.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();
  }
}
