using Contracts;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RawClient;


internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.MapRemote("http://localhost:8080", c =>
        {
            c.Register<CreateOrderCommand, CreateOrderResult>();
        });
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
                    .ConfigureHostOptions(options =>
                    {
                        options.ShutdownTimeout = TimeSpan.FromSeconds(30);
                        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                        // options.HostingEnvironment = Environments.Production;
                    })
                    .ConfigureHostConfiguration(configure =>
                    {
                        // configure.AddJsonFile("appsettings.json");
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddLogging(configure =>
                        {
                            // configure.ClearProviders();
                            // configure.AddNLog("nlog.config");
                        });
                        services.AddHostedService<RemoteWorkerJob>();
                    })
                    .ConfigureAppConfiguration(app =>
                    {
                    });

        return builder;
    }
}

