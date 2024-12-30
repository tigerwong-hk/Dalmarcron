using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public interface IDalmarcronSchedulerQueryService
{
    Task<Result<JobPublishedTransactionDetailOutputDto, ErrorDetail>> GetJobPublishedTransactionDetailAsync(GetJobPublishedTransactionDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<JobPublishedTransactionOutputDto>, ErrorDetail>> GetJobPublishedTransactionListAsync(GetJobPublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<JobUnpublishedTransactionDetailOutputDto, ErrorDetail>> GetJobUnpublishedTransactionDetailAsync(GetJobUnpublishedTransactionDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<JobUnpublishedTransactionOutputDto>, ErrorDetail>> GetJobUnpublishedTransactionListAsync(GetJobUnpublishedTransactionListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<PublishedJobDetailOutputDto, ErrorDetail>> GetPublishedJobDetailAsync(GetPublishedJobDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ScheduledJobDetailOutputDto, ErrorDetail>> GetScheduledJobDetailAsync(GetScheduledJobDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<ScheduledJobOutputDto>, ErrorDetail>> GetScheduledJobListAsync(GetScheduledJobListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ScheduledJobSecretsOutputDto, ErrorDetail>> GetScheduledJobSecretsAsync(GetScheduledJobSecretsInputDto inputDto, CancellationToken cancellationToken = default);
}
