using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public interface IDalmarcronSchedulerQueryService
{
    Task<Result<ScheduledJobDetailOutputDto, ErrorDetail>> GetScheduledJobDetailAsync(GetScheduledJobDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<ScheduledJobOutputDto>, ErrorDetail>> GetScheduledJobListAsync(GetScheduledJobListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ScheduledJobSecretsOutputDto, ErrorDetail>> GetScheduledJobSecretsAsync(GetScheduledJobSecretsInputDto inputDto, CancellationToken cancellationToken = default);
}
