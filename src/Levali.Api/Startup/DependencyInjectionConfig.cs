using Levali.Core;
using Levali.Infra;
using Serilog.Core;

namespace Levali.Api;

public static class DependencyInjectionConfig
{
    public static void ConfigureDI(this IServiceCollection services, ConfigurationManager configuration, Logger logger)
    {               
        services.AddSingleton<Serilog.ILogger>(logger);

        services.AddTransient(d => new DbSession(configuration["DbConnectionString"]));
        // services.AddTransient<IPublisher>(p => new RedisPubSub(configuration["RedisConnectionString"], logger));
        // services.AddTransient<ISubscriber>(s => new RedisPubSub(configuration["RedisConnectionString"], logger));

        services.AddTransient<IEnqueueer>(e => new RedisQueue(configuration["RedisConnectionString"], logger));
        services.AddTransient<IDequeueer>(d => new RedisQueue(configuration["RedisConnectionString"], logger));

        services.AddTransient<IUserRepository, UserMssqlRepository>();
        services.AddTransient<IShortUrlRepository, ShortUrlMssqlRepository>();
        services.AddTransient(g => new JwtTokenGenerator(configuration["SecretKey"]));
        services.AddTransient<ShortenUrlService>();
        services.AddTransient(s => new Lazy<ShortenUrlService>(s.GetRequiredService<ShortenUrlService>));
        services.AddTransient<EnlargeShortUrlService>();
        services.AddTransient(e => new Lazy<EnlargeShortUrlService>(e.GetRequiredService<EnlargeShortUrlService>));
        services.AddScoped<Notification>();

        // services.AddHostedService<UpdateAnalyticsService>();
        // services.AddHostedService<LazyRemoveService>();
    }
}
