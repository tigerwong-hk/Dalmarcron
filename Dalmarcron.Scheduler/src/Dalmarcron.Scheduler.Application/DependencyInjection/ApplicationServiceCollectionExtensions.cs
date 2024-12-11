using Dalmarcron.Scheduler.Application.Mapping;
using Dalmarcron.Scheduler.Application.Services.ApplicationServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Microsoft.Extensions.DependencyInjection;

namespace Dalmarcron.Scheduler.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        _ = services.AddSingleton(_ => new MapperConfigurations().CreateMapper());

        _ = services.AddScoped<IScheduledJobDataService, ScheduledJobDataService>();

        _ = services.AddScoped<IDalmarcronSchedulerQueryService, DalmarcronSchedulerQueryService>();
        _ = services.AddScoped<IDalmarcronSchedulerCommandService, DalmarcronSchedulerCommandService>();

        return services;
    }
}
