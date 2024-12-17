using AutoMapper;
using Dalmarcron.Scheduler.Application.Mappers;
using Dalmarcron.Scheduler.Application.Options;
using Dalmarcron.Scheduler.Application.Services.AwsServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Dalmarcron.Scheduler.Core.Constants;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerCommandService : ApplicationCommandServiceBase, IDalmarcronSchedulerCommandService
{
    private readonly IMapper _mapper;
    private readonly SchedulerOptions _schedulerOptions;
    private readonly IScheduledJobDataService _scheduledJobDataService;
    private readonly IAwsSystemsManagerService _awsSystemsManagerService;

    public DalmarcronSchedulerCommandService(
        IMapper mapper,
        IOptions<SchedulerOptions> schedulerOptions,
        IScheduledJobDataService scheduledJobDataService,
        IAwsSystemsManagerService awsSystemsManagerService) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _schedulerOptions = Guard.NotNull(schedulerOptions, nameof(schedulerOptions)).Value;
        _schedulerOptions.Validate();

        _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));
        _awsSystemsManagerService = Guard.NotNull(awsSystemsManagerService, nameof(awsSystemsManagerService));
    }

    #region ScheduledJob
    public async Task<Result<Guid, ErrorDetail>> CreateScheduledJobAsync(CreateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob scheduledJob = _mapper.Map<ScheduledJob>(
            inputDto,
            opts =>
                opts.Items[MapperItemKeys.SymmetricEncryptionSecretKey] = _schedulerOptions.SymmetricEncryptionSecretKey
        );
        scheduledJob.Validate();

        _ = await _scheduledJobDataService.CreateAsync(scheduledJob, auditDetail, cancellationToken);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> DeleteScheduledJobAsync(DeleteScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(
            inputDto.ScheduledJobId,
            false,
            cancellationToken
        );
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        scheduledJob.IsDeleted = true;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> PublishScheduledJobAsync(PublishScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(
            inputDto.ScheduledJobId,
            false,
            cancellationToken
        );
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        ScheduledJobSecretsOutputDto outputDto = _mapper.Map<ScheduledJobSecretsOutputDto>(
            scheduledJob,
            opts =>
                opts.Items[MapperItemKeys.SymmetricEncryptionSecretKey] = _schedulerOptions.SymmetricEncryptionSecretKey
        );

        await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{outputDto.ScheduledJobId}/{ScheduledJobParameterKey.ApiUrl}", outputDto.ApiUrl);

        if ((outputDto.ApiHeaders?.Count ?? 0) > 0)
        {
            string apiHeadersJsonString = JsonSerializer.Serialize(outputDto.ApiHeaders);
            await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{outputDto.ScheduledJobId}/{ScheduledJobParameterKey.ApiHeaders}", apiHeadersJsonString);
        }

        if (!string.IsNullOrWhiteSpace(outputDto.ApiJsonBody))
        {
            await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{outputDto.ScheduledJobId}/{ScheduledJobParameterKey.ApiJsonBody}", outputDto.ApiJsonBody);
        }

        if (!string.IsNullOrWhiteSpace(outputDto.Oauth2ClientId))
        {
            await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{outputDto.ScheduledJobId}/{ScheduledJobParameterKey.Oauth2ClientId}", outputDto.Oauth2ClientId);
        }

        if (!string.IsNullOrWhiteSpace(outputDto.Oauth2ClientSecret))
        {
            await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{outputDto.ScheduledJobId}/{ScheduledJobParameterKey.Oauth2ClientSecret}", outputDto.Oauth2ClientSecret);
        }

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> UnpublishScheduledJobAsync(UnpublishScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(
            inputDto.ScheduledJobId,
            false,
            cancellationToken
        );
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        List<string> deleteParameterNames = [$"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{ScheduledJobParameterKey.ApiUrl}"];

        if (!string.IsNullOrWhiteSpace(scheduledJob.ApiHeaders))
        {
            deleteParameterNames.Add($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{ScheduledJobParameterKey.ApiHeaders}");
        }

        if (!string.IsNullOrWhiteSpace(scheduledJob.ApiJsonBody))
        {
            deleteParameterNames.Add($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{ScheduledJobParameterKey.ApiJsonBody}");
        }

        if (!string.IsNullOrWhiteSpace(scheduledJob.Oauth2ClientId))
        {
            deleteParameterNames.Add($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{ScheduledJobParameterKey.Oauth2ClientId}");
        }

        if (!string.IsNullOrWhiteSpace(scheduledJob.Oauth2ClientSecret))
        {
            deleteParameterNames.Add($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{ScheduledJobParameterKey.Oauth2ClientSecret}");
        }

        await _awsSystemsManagerService.DeleteParametersAsync(deleteParameterNames);

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> UpdateScheduledJobAsync(UpdateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(
            inputDto.ScheduledJobId,
            false,
            cancellationToken
        );
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        scheduledJob = _mapper.Map(
            inputDto,
            scheduledJob,
            opts =>
                opts.Items[MapperItemKeys.SymmetricEncryptionSecretKey] = _schedulerOptions.SymmetricEncryptionSecretKey
        );
        scheduledJob.Validate();

        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }
    #endregion ScheduledJob
}
