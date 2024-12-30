using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Contexts;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public class JobUnpublishedTransactionDataService(DalmarcronSchedulerDbContext dbContext)
    : ReadOnlyDataServiceBase<DalmarcronSchedulerDbContext, JobUnpublishedTransaction>(dbContext), IJobUnpublishedTransactionDataService
{
    public async Task<JobUnpublishedTransaction?> GetJobUnpublishedTransactionDetailAsync(Guid jobUnpublishedTransactionId, CancellationToken cancellationToken = default)
    {
        IQueryable<JobUnpublishedTransaction> queryable = DbContext.JobUnpublishedTransactions
            .Where(x => x.JobUnpublishedTransactionId == jobUnpublishedTransactionId);

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsePagination<JobUnpublishedTransaction>> GetJobUnpublishedTransactionListAsync(GetJobUnpublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IQueryable<JobUnpublishedTransaction> queryable = DbContext.JobUnpublishedTransactions
            .Where(GetJobUnpublishedTransactionListFilter(inputDto))
            .OrderByDescending(x => x.CreatedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        List<JobUnpublishedTransaction> data = await queryable
            .Skip(inputDto.GetSkipCount())
            .Take(inputDto.PageSize)
            .ToListAsync(cancellationToken);

        return new ResponsePagination<JobUnpublishedTransaction>(data, filteredCount, inputDto.PageNumber, inputDto.PageSize);
    }

    protected static Expression<Func<JobUnpublishedTransaction, bool>> GetJobUnpublishedTransactionListFilter(GetJobUnpublishedTransactionListInputDto inputDto)
    {
        return inputDto switch
        {
            GetJobUnpublishedTransactionListInputDto input when inputDto.ScheduledJobId is null =>
                _ => true,

            GetJobUnpublishedTransactionListInputDto input when inputDto.ScheduledJobId is not null =>
                x => x.ScheduledJobId == inputDto.ScheduledJobId,

            _ => throw new InvalidOperationException()
        };
    }
}
