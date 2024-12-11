using Dalmarkit.Common.Dtos.Components;
using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace Dalmarcron.Scheduler.Core.Dtos.Inputs;

public class GetScheduledJobListInputDto : PaginationContext
{
    [StringLength(64, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string? JobName { get; set; }
}
