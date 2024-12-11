using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Errors;

namespace Dalmarcron.Scheduler.Application.Services.ApplicationServices;

public interface IDalmarcronSchedulerCommandService
{
    Task<Result<Guid, ErrorDetail>> CreateScheduledJobAsync(CreateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> DeleteScheduledJobAsync(DeleteScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> UpdateScheduledJobAsync(UpdateScheduledJobInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);
}
