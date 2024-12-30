using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public interface IJobUnpublishedTransactionDataService : IReadOnlyDataServiceBase<JobUnpublishedTransaction>
{
    Task<JobUnpublishedTransaction?> GetJobUnpublishedTransactionDetailAsync(Guid jobUnpublishedTransactionId, CancellationToken cancellationToken = default);

    Task<ResponsePagination<JobUnpublishedTransaction>> GetJobUnpublishedTransactionListAsync(GetJobUnpublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<DateTime> GetLatestJobUnpublishedTransactionDateTimeAsync(Guid scheduledJobId, CancellationToken cancellationToken = default);
}
