using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace Dalmarcron.Scheduler.Core.Dtos.Inputs;

public class CreateScheduledJobInputDto : ScheduledJobInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(64, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string CreateRequestId { get; set; } = null!;
}
