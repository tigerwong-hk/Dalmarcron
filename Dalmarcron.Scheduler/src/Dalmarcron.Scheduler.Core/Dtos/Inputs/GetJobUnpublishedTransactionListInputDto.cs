using Dalmarkit.Common.Dtos.Components;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;

namespace Dalmarcron.Scheduler.Core.Dtos.Inputs;

public class GetJobUnpublishedTransactionListInputDto : PaginationContext
{
    [NotDefault(ErrorMessage = ErrorMessages.ModelStateErrors.ValueNotDefault)]
    public Guid? ScheduledJobId { get; set; }
}
