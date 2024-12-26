using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace Dalmarcron.Scheduler.Core.Dtos.Inputs;

public class GetJobPublishedTransactionDetailInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [NotDefault(ErrorMessage = ErrorMessages.ModelStateErrors.ValueNotDefault)]
    public Guid JobPublishedTransactionId { get; set; }
}
