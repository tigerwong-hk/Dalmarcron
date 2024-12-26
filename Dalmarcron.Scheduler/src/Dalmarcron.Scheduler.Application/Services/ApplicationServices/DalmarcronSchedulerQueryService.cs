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
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Microsoft.Extensions.Options;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerQueryService : ApplicationQueryServiceBase, IDalmarcronSchedulerQueryService
{
    private readonly IMapper _mapper;
    private readonly SchedulerOptions _schedulerOptions;
    private readonly IScheduledJobDataService _scheduledJobDataService;
    private readonly IJobPublishedTransactionDataService _jobPublishedTransactionDataService;
    private readonly IAwsLambdaService _awsLambdaService;

    public DalmarcronSchedulerQueryService(
        IMapper mapper,
        IOptions<SchedulerOptions> schedulerOptions,
        IScheduledJobDataService scheduledJobDataService,
        IJobPublishedTransactionDataService jobPublishedTransactionDataService,
        IAwsLambdaService awsLambdaService) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _schedulerOptions = Guard.NotNull(schedulerOptions, nameof(schedulerOptions)).Value;
        _schedulerOptions.Validate();

        _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));
        _jobPublishedTransactionDataService = Guard.NotNull(jobPublishedTransactionDataService, nameof(jobPublishedTransactionDataService));

        _awsLambdaService = Guard.NotNull(awsLambdaService, nameof(awsLambdaService));
    }

    #region JobPublishedTransaction
    public async Task<Result<JobPublishedTransactionDetailOutputDto, ErrorDetail>> GetJobPublishedTransactionDetailAsync(GetJobPublishedTransactionDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        JobPublishedTransaction? jobPublishedTransaction = await _jobPublishedTransactionDataService.GetJobPublishedTransactionDetailAsync(inputDto.JobPublishedTransactionId, cancellationToken);
        if (jobPublishedTransaction == null)
        {
            return Error<JobPublishedTransactionDetailOutputDto>(ErrorTypes.ResourceNotFoundFor, "JobPublishedTransaction", inputDto.JobPublishedTransactionId);
        }

        JobPublishedTransactionDetailOutputDto output = _mapper.Map<JobPublishedTransactionDetailOutputDto>(jobPublishedTransaction);

        return Ok(output);
    }

    public async Task<Result<ResponsePagination<JobPublishedTransactionOutputDto>, ErrorDetail>> GetJobPublishedTransactionListAsync(GetJobPublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ResponsePagination<JobPublishedTransaction> jobPublishedTransactionList = await _jobPublishedTransactionDataService.GetJobPublishedTransactionListAsync(inputDto, cancellationToken);
        IEnumerable<JobPublishedTransactionOutputDto> output = _mapper.Map<IEnumerable<JobPublishedTransactionOutputDto>>(jobPublishedTransactionList.Data);

        return Ok(new ResponsePagination<JobPublishedTransactionOutputDto>(output, jobPublishedTransactionList.FilteredCount, jobPublishedTransactionList.PageNumber, jobPublishedTransactionList.PageSize));
    }
    #endregion JobPublishedTransaction

    #region ScheduledJob
    public async Task<Result<PublishedJobDetailOutputDto, ErrorDetail>> GetPublishedJobDetailAsync(GetPublishedJobDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.GetScheduledJobDetailAsync(inputDto.ScheduledJobId, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<PublishedJobDetailOutputDto>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }
        if (scheduledJob.PublicationState != PublicationState.Published)
        {
            return Error<PublishedJobDetailOutputDto>(ErrorTypes.BadRequestDetails, $"ScheduledJob must be published: {inputDto.ScheduledJobId}");
        }

        string functionName = _schedulerOptions.GetLambdaFunctionName(scheduledJob.ScheduledJobId);
        FunctionConfig functionConfig = await _awsLambdaService.GetFunctionAsync(functionName);

        PublishedJobDetailOutputDto output = _mapper.Map<PublishedJobDetailOutputDto>(
            scheduledJob,
            opts =>
                opts.Items[MapperItemKeys.FunctionConfig] = functionConfig
        );

        return Ok(output);
    }

    public async Task<Result<ScheduledJobDetailOutputDto, ErrorDetail>> GetScheduledJobDetailAsync(GetScheduledJobDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.GetScheduledJobDetailAsync(inputDto.ScheduledJobId, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<ScheduledJobDetailOutputDto>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }

        ScheduledJobDetailOutputDto output = _mapper.Map<ScheduledJobDetailOutputDto>(scheduledJob);

        return Ok(output);
    }

    public async Task<Result<ResponsePagination<ScheduledJobOutputDto>, ErrorDetail>> GetScheduledJobListAsync(GetScheduledJobListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ResponsePagination<ScheduledJob> scheduledJobList = await _scheduledJobDataService.GetScheduledJobListAsync(inputDto, cancellationToken);
        IEnumerable<ScheduledJobOutputDto> output = _mapper.Map<IEnumerable<ScheduledJobOutputDto>>(scheduledJobList.Data);

        return Ok(new ResponsePagination<ScheduledJobOutputDto>(output, scheduledJobList.FilteredCount, scheduledJobList.PageNumber, scheduledJobList.PageSize));
    }

    public async Task<Result<ScheduledJobSecretsOutputDto, ErrorDetail>> GetScheduledJobSecretsAsync(GetScheduledJobSecretsInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.GetScheduledJobDetailAsync(inputDto.ScheduledJobId, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<ScheduledJobSecretsOutputDto>(ErrorTypes.ResourceNotFoundFor, "ScheduledJob", inputDto.ScheduledJobId);
        }

        ScheduledJobSecretsOutputDto output = _mapper.Map<ScheduledJobSecretsOutputDto>(
            scheduledJob,
            opts =>
                opts.Items[MapperItemKeys.SymmetricEncryptionSecretKey] = _schedulerOptions.SymmetricEncryptionSecretKey
        );

        return Ok(output);
    }
    #endregion ScheduledJob
}
