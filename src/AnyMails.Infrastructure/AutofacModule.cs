using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace AnyMails.Infrastructure;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Default registration is by scope
        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

        builder.RegisterModule<Application.AutofacModule>();
    }
}
