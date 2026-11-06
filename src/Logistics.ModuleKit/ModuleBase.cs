using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.ModuleKit;

// One class is both the hosting startup (runs while the host is being built)
// and the startup filter (runs while the request pipeline is being built).
public abstract class ModuleBase : IHostingStartup, IStartupFilter
{
    void IHostingStartup.Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
        builder.ConfigureServices((ctx, services) =>
        {
            services.AddSingleton<IStartupFilter>(this);
            ConfigureServices(ctx, services);
        });
    }

    Action<IApplicationBuilder> IStartupFilter.Configure(Action<IApplicationBuilder> next) => app =>
    {
        next(app);      // host pipeline first (it calls UseRouting)
        Configure(app); // then this module's middleware and endpoints
    };

    protected virtual void ConfigureAppConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config) { }

    protected virtual void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services) { }

    protected virtual void Configure(IApplicationBuilder app) { }
}
