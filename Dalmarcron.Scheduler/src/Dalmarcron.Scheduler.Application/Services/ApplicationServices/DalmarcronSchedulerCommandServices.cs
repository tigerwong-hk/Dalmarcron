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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerCommandService : ApplicationCommandServiceBase, IDalmarcronSchedulerCommandService
{
    private readonly IMapper _mapper;
    private readonly SchedulerOptions _schedulerOptions;
    private readonly IScheduledJobDataService _scheduledJobDataService;
    private readonly IAwsSystemsManagerService _awsSystemsManagerService;
    private readonly ILogger _logger;

    public DalmarcronSchedulerCommandService(
        IMapper mapper,
        IOptions<SchedulerOptions> schedulerOptions,
        IScheduledJobDataService scheduledJobDataService,
        IAwsSystemsManagerService awsSystemsManagerService,
        ILogger<DalmarcronSchedulerCommandService> logger) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _schedulerOptions = Guard.NotNull(schedulerOptions, nameof(schedulerOptions)).Value;
        _schedulerOptions.Validate();

        _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));
        _awsSystemsManagerService = Guard.NotNull(awsSystemsManagerService, nameof(awsSystemsManagerService));

        _logger = Guard.NotNull(logger, nameof(logger));
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
        scheduledJob.PublicationState = PublicationState.Unpublished;
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
            return Error<Guid>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
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
            return Error<Guid>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }
        if (scheduledJob.PublicationState != PublicationState.Unpublished)
        {
            return Error<Guid>(ErrorTypes.BadRequestDetails, $"ScheduledJob must be unpublished: {inputDto.ScheduledJobId}");
        }

        scheduledJob.PublicationState = PublicationState.PendingPublish;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        ScheduledJobSecretsOutputDto outputDto = _mapper.Map<ScheduledJobSecretsOutputDto>(
            scheduledJob,
            opts =>
                opts.Items[MapperItemKeys.SymmetricEncryptionSecretKey] = _schedulerOptions.SymmetricEncryptionSecretKey
        );

        List<string> deleteParameterNames = [];

        Result<Guid, ErrorDetail> apiUrlResult = await SetSecretParameterAsync(scheduledJob, auditDetail, ScheduledJobParameterKey.ApiUrl, outputDto.ApiUrl, deleteParameterNames, cancellationToken);
        if (apiUrlResult.HasError)
        {
            return apiUrlResult;
        }

        if ((outputDto.ApiHeaders?.Count ?? 0) > 0)
        {
            Result<Guid, ErrorDetail> result = await SetSecretParameterAsync(scheduledJob, auditDetail, ScheduledJobParameterKey.ApiHeaders, outputDto.ApiHeaders!, deleteParameterNames, cancellationToken);
            if (result.HasError)
            {
                return result;
            }
        }

        if (!string.IsNullOrWhiteSpace(outputDto.ApiJsonBody))
        {
            Result<Guid, ErrorDetail> result = await SetSecretParameterAsync(scheduledJob, auditDetail, ScheduledJobParameterKey.ApiJsonBody, outputDto.ApiJsonBody, deleteParameterNames, cancellationToken);
            if (result.HasError)
            {
                return result;
            }
        }

        if (!string.IsNullOrWhiteSpace(outputDto.Oauth2ClientId))
        {
            Result<Guid, ErrorDetail> result = await SetSecretParameterAsync(scheduledJob, auditDetail, ScheduledJobParameterKey.Oauth2ClientId, outputDto.Oauth2ClientId, deleteParameterNames, cancellationToken);
            if (result.HasError)
            {
                return result;
            }
        }

        if (!string.IsNullOrWhiteSpace(outputDto.Oauth2ClientSecret))
        {
            Result<Guid, ErrorDetail> result = await SetSecretParameterAsync(scheduledJob, auditDetail, ScheduledJobParameterKey.Oauth2ClientSecret, outputDto.Oauth2ClientSecret, deleteParameterNames, cancellationToken);
            if (result.HasError)
            {
                return result;
            }
        }

        scheduledJob.PublicationState = PublicationState.Published;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

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
            return Error<Guid>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }
        if (scheduledJob.PublicationState != PublicationState.Published)
        {
            return Error<Guid>(ErrorTypes.BadRequestDetails, $"ScheduledJob must be published: {inputDto.ScheduledJobId}");
        }

        scheduledJob.PublicationState = PublicationState.PendingUnpublish;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

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

        try
        {
            await _awsSystemsManagerService.DeleteParametersAsync(deleteParameterNames);
        }
        catch (Exception ex)
        {
            _logger.DeleteSecretParametersError(deleteParameterNames, ex.Message, ex.InnerException?.Message, ex.StackTrace);

            scheduledJob.PublicationState = PublicationState.Published;
            _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
            _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

            return Error<Guid>(ErrorTypes.ServerError);
        }

        scheduledJob.PublicationState = PublicationState.Unpublished;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

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
            return Error<Guid>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }
        if (scheduledJob.PublicationState != PublicationState.Unpublished)
        {
            return Error<Guid>(ErrorTypes.BadRequestDetails, $"ScheduledJob must be unpublished: {inputDto.ScheduledJobId}");
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

    private async Task<Result<Guid, ErrorDetail>> SetSecretParameterAsync(ScheduledJob scheduledJob,
        AuditDetail auditDetail,
        string parameterKey,
        object parameter,
        List<string> deleteParameterNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string parameterString = parameter is string param ? param : JsonSerializer.Serialize(parameter);
            await _awsSystemsManagerService.SetSecretParameterAsync($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{parameterKey}", parameterString);
            deleteParameterNames.Add($"{_schedulerOptions.SsmParametersPathPrefix}/{scheduledJob.ScheduledJobId}/{parameterKey}");

            return Ok(scheduledJob.ScheduledJobId);
        }
        catch (Exception ex)
        {
            _logger.SetSecretParameterError(parameterKey, ex.Message, ex.InnerException?.Message, ex.StackTrace);

            if (deleteParameterNames.Count > 0)
            {
                await _awsSystemsManagerService.DeleteParametersAsync(deleteParameterNames);
            }

            scheduledJob.PublicationState = PublicationState.Unpublished;
            _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
            _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

            return Error<Guid>(ErrorTypes.ServerError);
        }
    }
}

public static partial class DalmarcronSchedulerCommandServiceLogs
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Set secret parameter error for `{ParameterName}` with message `{Message}` and inner exception `{InnerException}`: {StackTrace}")]
    public static partial void SetSecretParameterError(
        this ILogger logger, string parameterName, string message, string? innerException, string? stackTrace);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Delete secret parameters error for `{deleteParameterNames}` with message `{Message}` and inner exception `{InnerException}`: {StackTrace}")]
    public static partial void DeleteSecretParametersError(
        this ILogger logger, List<string> deleteParameterNames, string message, string? innerException, string? stackTrace);
}
