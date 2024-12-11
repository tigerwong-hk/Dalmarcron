using Dalmarcron.Scheduler.Core.Constants;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace Dalmarcron.Scheduler.Core.Dtos.Inputs;

public class ScheduledJobInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    public ApiMethod ApiMethod { get; set; }

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    public ApiType ApiType { get; set; }

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(2048, MinimumLength = 7, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    [Url(ErrorMessage = ErrorMessages.ModelStateErrors.UrlInvalid)]
    public string ApiUrl { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(250, MinimumLength = 11, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string CronExpression { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(64, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string JobName { get; set; } = null!;

    [MaxLength(20, ErrorMessage = ErrorMessages.ModelStateErrors.ElementsTooMany)]
    [NotDefault(ErrorMessage = ErrorMessages.ModelStateErrors.ValueNotDefault)]
    public IDictionary<string, string>? ApiHeaders { get; set; }

    [StringLength(64, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string? ApiIdempotencyKey { get; set; }

    public object? ApiJsonBody { get; set; }

    [StringLength(1024, MinimumLength = 7, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    [Url(ErrorMessage = ErrorMessages.ModelStateErrors.UrlInvalid)]
    public string? Oauth2BaseUri { get; set; }

    [StringLength(128, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string? Oauth2ClientId { get; set; }

    [MaxLength(50, ErrorMessage = ErrorMessages.ModelStateErrors.ElementsTooMany)]
    [NotDefault(ErrorMessage = ErrorMessages.ModelStateErrors.ValueNotDefault)]
    public IEnumerable<string>? Oauth2ClientScopes { get; set; }

    [StringLength(64, MinimumLength = 1, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string? Oauth2ClientSecret { get; set; }
}
