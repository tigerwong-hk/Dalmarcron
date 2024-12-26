using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Contexts;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public class JobPublishedTransactionDataService(DalmarcronSchedulerDbContext dbContext)
    : ReadOnlyDataServiceBase<DalmarcronSchedulerDbContext, JobPublishedTransaction>(dbContext), IJobPublishedTransactionDataService
{
    public async Task<JobPublishedTransaction?> GetJobPublishedTransactionDetailAsync(Guid jobPublishedTransactionId, CancellationToken cancellationToken = default)
    {
        IQueryable<JobPublishedTransaction> queryable = DbContext.JobPublishedTransactions
            .Where(x => x.JobPublishedTransactionId == jobPublishedTransactionId);

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsePagination<JobPublishedTransaction>> GetJobPublishedTransactionListAsync(GetJobPublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IQueryable<JobPublishedTransaction> queryable = DbContext.JobPublishedTransactions
            .Where(GetJobPublishedTransactionListFilter(inputDto))
            .OrderByDescending(x => x.CreatedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        List<JobPublishedTransaction> data = await queryable
            .Skip(inputDto.GetSkipCount())
            .Take(inputDto.PageSize)
            .ToListAsync(cancellationToken);

        return new ResponsePagination<JobPublishedTransaction>(data, filteredCount, inputDto.PageNumber, inputDto.PageSize);
    }

    protected static Expression<Func<JobPublishedTransaction, bool>> GetJobPublishedTransactionListFilter(GetJobPublishedTransactionListInputDto inputDto)
    {
        return inputDto switch
        {
            GetJobPublishedTransactionListInputDto input when inputDto.ScheduledJobId is null =>
                _ => true,

            GetJobPublishedTransactionListInputDto input when inputDto.ScheduledJobId is not null =>
                x => x.ScheduledJobId == inputDto.ScheduledJobId,

            _ => throw new InvalidOperationException()
        };
    }
}
