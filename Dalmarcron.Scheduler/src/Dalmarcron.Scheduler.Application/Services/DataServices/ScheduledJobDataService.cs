using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Contexts;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarcron.Scheduler.Application.Services.DataServices;

public class ScheduledJobDataService(DalmarcronSchedulerDbContext dbContext)
    : ReadWriteDataServiceBase<DalmarcronSchedulerDbContext, ScheduledJob>(dbContext), IScheduledJobDataService
{
    public async Task<ScheduledJob?> GetScheduledJobDetailAsync(Guid scheduledJobId, CancellationToken cancellationToken = default)
    {
        IQueryable<ScheduledJob> queryable = DbContext.ScheduledJobs
            .Where(x => !x.IsDeleted && x.ScheduledJobId == scheduledJobId);

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsePagination<ScheduledJob>> GetScheduledJobListAsync(GetScheduledJobListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IQueryable<ScheduledJob> queryable = DbContext.ScheduledJobs
            .Where(GetScheduledJobListFilter(inputDto))
            .OrderByDescending(x => x.ModifiedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        List<ScheduledJob> data = await queryable
            .Skip(inputDto.GetSkipCount())
            .Take(inputDto.PageSize)
            .ToListAsync(cancellationToken);

        return new ResponsePagination<ScheduledJob>(data, filteredCount, inputDto.PageNumber, inputDto.PageSize);
    }

    protected static Expression<Func<ScheduledJob, bool>> GetScheduledJobListFilter(GetScheduledJobListInputDto inputDto)
    {
        return inputDto switch
        {
            GetScheduledJobListInputDto input when string.IsNullOrWhiteSpace(inputDto.JobName) =>
                x => !x.IsDeleted,

            GetScheduledJobListInputDto input when !string.IsNullOrWhiteSpace(inputDto.JobName) =>
                x => !x.IsDeleted && x.JobName.Contains(inputDto.JobName.Trim()),

            _ => throw new InvalidOperationException()
        };
    }
}
