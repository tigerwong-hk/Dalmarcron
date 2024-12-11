using AutoMapper;
using Dalmarcron.Scheduler.Application.Options;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Microsoft.Extensions.Options;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerCommandService : ApplicationCommandServiceBase, IDalmarcronSchedulerCommandService
{
    private readonly IMapper _mapper;
    private readonly SchedulerOptions _schedulerOptions;
    private readonly IScheduledJobDataService _scheduledJobDataService;

    public DalmarcronSchedulerCommandService(IMapper mapper,
        IOptions<SchedulerOptions> schedulerOptions,
        IScheduledJobDataService scheduledJobDataService) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _schedulerOptions = Guard.NotNull(schedulerOptions, nameof(schedulerOptions)).Value;
        _schedulerOptions.Validate();

        _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));
    }

    #region ScheduledJob
    public async Task<Result<Guid, ErrorDetail>> CreateScheduledJobAsync(CreateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob scheduledJob = _mapper.Map<ScheduledJob>(inputDto);
        _ = await _scheduledJobDataService.CreateAsync(scheduledJob, auditDetail, cancellationToken);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> DeleteScheduledJobAsync(DeleteScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(inputDto.ScheduledJobId, false, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        scheduledJob.IsDeleted = true;
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }

    public async Task<Result<Guid, ErrorDetail>> UpdateScheduledJobAsync(UpdateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.FindEntityIdAsync(inputDto.ScheduledJobId, false, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
        }

        scheduledJob = _mapper.Map(inputDto, scheduledJob);
        _ = _scheduledJobDataService.Update(scheduledJob, auditDetail);
        _ = await _scheduledJobDataService.SaveChangesAsync(cancellationToken);

        return Ok(scheduledJob.ScheduledJobId);
    }
    #endregion ScheduledJob
}
