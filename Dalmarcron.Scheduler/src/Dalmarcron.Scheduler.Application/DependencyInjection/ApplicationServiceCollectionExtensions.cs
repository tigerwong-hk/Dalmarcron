using Amazon.CloudWatchEvents;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda;
using Amazon.SimpleSystemsManagement;
using Dalmarcron.Scheduler.Application.Mappers;
using Dalmarcron.Scheduler.Application.Services.ApplicationServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Dalmarkit.Cloud.Aws.Services;
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
        _ = services.AddScoped<IJobPublishedTransactionDataService, JobPublishedTransactionDataService>();
        _ = services.AddScoped<IJobUnpublishedTransactionDataService, JobUnpublishedTransactionDataService>();

        _ = services.AddScoped<IDalmarcronSchedulerQueryService, DalmarcronSchedulerQueryService>();
        _ = services.AddScoped<IDalmarcronSchedulerCommandService, DalmarcronSchedulerCommandService>();

        return services;
    }

    public static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration config)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        AWSOptions awsOptions = config.GetAWSOptions();

        _ = services.AddSingleton(_ => new AmazonCloudWatchEventsClient(awsOptions.Region));
        _ = services.AddSingleton(_ => new AmazonLambdaClient(awsOptions.Region));
        _ = services.AddSingleton(_ => new AmazonSimpleSystemsManagementClient(awsOptions.Region));

        _ = services.AddScoped<IAwsCloudWatchEventsService, AwsCloudWatchEventsService>();
        _ = services.AddScoped<IAwsLambdaService, AwsLambdaService>();
        _ = services.AddScoped<IAwsSystemsManagerService, AwsSystemsManagerService>();

        return services;
    }
}
