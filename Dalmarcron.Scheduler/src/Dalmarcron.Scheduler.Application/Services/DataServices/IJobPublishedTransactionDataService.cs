using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public interface IJobPublishedTransactionDataService : IReadOnlyDataServiceBase<JobPublishedTransaction>
{
    Task<JobPublishedTransaction?> GetJobPublishedTransactionDetailAsync(Guid jobPublishedTransactionId, CancellationToken cancellationToken = default);

    Task<ResponsePagination<JobPublishedTransaction>> GetJobPublishedTransactionListAsync(GetJobPublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default);
}
