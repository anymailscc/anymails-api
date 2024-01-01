using AnyMails.Api;
using Autofac;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(e => e.RegisterAssemblyModules(typeof(AutofacModule).Assembly));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
