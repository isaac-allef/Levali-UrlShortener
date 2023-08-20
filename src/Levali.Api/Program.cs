using Levali.Api;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAuth(builder.Configuration);

var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
                    .Enrich.FromLogContext()
                    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services));

builder.Services.ConfigureDI(builder.Configuration, logger);

builder.Services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (httpContext.Response.StatusCode >= 400)
        {
            return LogEventLevel.Error;
        }
        return LogEventLevel.Information;
    };
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "It's up!");

app.Run();
