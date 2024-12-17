using AutoMapper;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarcron.Scheduler.Application.Options;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarcron.Scheduler.Application.Mappers;
using Microsoft.Extensions.Options;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerQueryService : ApplicationQueryServiceBase, IDalmarcronSchedulerQueryService
{
    private readonly IMapper _mapper;
    private readonly SchedulerOptions _schedulerOptions;
    private readonly IScheduledJobDataService _scheduledJobDataService;

    public DalmarcronSchedulerQueryService(
        IMapper mapper,
        IOptions<SchedulerOptions> schedulerOptions,
        IScheduledJobDataService scheduledJobDataService) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _schedulerOptions = Guard.NotNull(schedulerOptions, nameof(schedulerOptions)).Value;
        _schedulerOptions.Validate();

        _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));
    }

    #region ScheduledJob
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
