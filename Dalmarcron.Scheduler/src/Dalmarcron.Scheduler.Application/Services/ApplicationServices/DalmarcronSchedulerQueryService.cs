using AutoMapper;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarcron.Scheduler.Application.Services.DataServices;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public class DalmarcronSchedulerQueryService(IMapper mapper,
    IScheduledJobDataService scheduledJobDataService) : ApplicationQueryServiceBase(mapper), IDalmarcronSchedulerQueryService
{
    private readonly IMapper _mapper = Guard.NotNull(mapper, nameof(mapper));
    private readonly IScheduledJobDataService _scheduledJobDataService = Guard.NotNull(scheduledJobDataService, nameof(scheduledJobDataService));

    #region ScheduledJob
    public async Task<Result<ScheduledJobDetailOutputDto, ErrorDetail>> GetScheduledJobDetailAsync(GetScheduledJobDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ScheduledJob? scheduledJob = await _scheduledJobDataService.GetScheduledJobDetailAsync(inputDto.ScheduledJobId, cancellationToken);
        if (scheduledJob == null)
        {
            return Error<ScheduledJobDetailOutputDto>(ErrorTypes.ResourceNotFound, "ScheduledJob", inputDto.ScheduledJobId);
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
    #endregion ScheduledJob
}
