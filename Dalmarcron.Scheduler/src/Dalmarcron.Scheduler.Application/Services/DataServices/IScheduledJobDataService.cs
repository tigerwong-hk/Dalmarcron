using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public interface IScheduledJobDataService : IReadWriteDataServiceBase<ScheduledJob>
{
    Task<ScheduledJob?> GetScheduledJobDetailAsync(Guid scheduledJobId, CancellationToken cancellationToken = default);

    Task<ResponsePagination<ScheduledJob>> GetScheduledJobListAsync(GetScheduledJobListInputDto inputDto, CancellationToken cancellationToken = default);
}
