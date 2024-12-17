using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleSystemsManagement;
using Dalmarcron.Scheduler.Application.Mappers;
using Dalmarcron.Scheduler.Application.Services.ApplicationServices;
using Dalmarcron.Scheduler.Application.Services.AwsServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dalmarcron.Scheduler.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        _ = services.AddSingleton(_ => new MapperConfigurations().CreateMapper());

        _ = services.AddCloudServices(config);

        _ = services.AddScoped<IScheduledJobDataService, ScheduledJobDataService>();

        _ = services.AddScoped<IDalmarcronSchedulerQueryService, DalmarcronSchedulerQueryService>();
        _ = services.AddScoped<IDalmarcronSchedulerCommandService, DalmarcronSchedulerCommandService>();

        return services;
    }

    public static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration config)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        AWSOptions awsOptions = config.GetAWSOptions();

        _ = services.AddSingleton(_ => new AmazonSimpleSystemsManagementClient(awsOptions.Region));

        _ = services.AddScoped<IAwsSystemsManagerService, AwsSystemsManagerService>();

        return services;
    }
}
