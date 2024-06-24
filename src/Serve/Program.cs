using NLog.Extensions.Logging;

namespace Serve;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddHandlerServer();
        builder.Services.AddCors();
        builder.Services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddNLog("nlog.config");
        });
        var app = builder.Build();

        app.UseGrpcWeb(new() { DefaultEnabled = true });
        app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.MapHandlers(h =>
        {
            h.Register<CreateOrderCommand, CreateOrderHandler, CreateOrderResult>();
            h.RegisterServerStream<StatusStreamCommand, StatusUpdateHandler, StatusUpdate>();
            h.RegisterClientStream<CurrentPosition, PositionProgressHandler, ProgressReport>();
        });

        await app.RunAsync();
    }
}