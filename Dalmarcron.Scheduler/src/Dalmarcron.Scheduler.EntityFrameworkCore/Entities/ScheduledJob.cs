using AutoMapper.Configuration.Annotations;
using Dalmarcron.Scheduler.Core.Constants;
using Dalmarkit.Common.Entities.BaseEntities;
using Dalmarkit.Common.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dalmarcron.Scheduler.EntityFrameworkCore.Entities;

public class ScheduledJob : ReadWriteEntityBase
{
    [Key]
    [Required]
    [Ignore]
    public Guid ScheduledJobId { get; set; }

    [Required]
    [MaxLength(20)]
    public ApiMethod ApiMethod { get; set; }

    [Required]
    [MaxLength(20)]
    public ApiType ApiType { get; set; }

    [Required]
    public string ApiUrl { get; set; } = null!;

    [Required]
    public string CronExpression { get; set; } = null!;

    [Required]
    public string JobName { get; set; } = null!;

    public string? ApiHeaders { get; set; }

    public string? ApiIdempotencyKey { get; set; }

    public string? ApiJsonBody { get; set; }

    public string? Oauth2BaseUri { get; set; }

    public string? Oauth2ClientId { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? Oauth2ClientScopes { get; set; }

    public string? Oauth2ClientSecret { get; set; }

    public void Validate()
    {
        switch (ApiMethod)
        {
            case ApiMethod.GET:
                if (!string.IsNullOrWhiteSpace(ApiIdempotencyKey))
                {
                    throw new ArgumentException("Not null", nameof(ApiIdempotencyKey));
                }
                if (!string.IsNullOrWhiteSpace(ApiJsonBody))
                {
                    throw new ArgumentException("Not null", nameof(ApiJsonBody));
                }
                break;
            case ApiMethod.POST:
                break;
            case ApiMethod.PUT:
                if (!string.IsNullOrWhiteSpace(ApiIdempotencyKey))
                {
                    throw new ArgumentException("Not null", nameof(ApiIdempotencyKey));
                }
                break;
            default:
                throw new ArgumentException($"Invalid ${ApiMethod}", nameof(ApiMethod));
        }

        switch (ApiType)
        {
            case ApiType.FETCH:
                if (!string.IsNullOrWhiteSpace(Oauth2BaseUri))
                {
                    throw new ArgumentException("Not null", nameof(Oauth2BaseUri));
                }
                if (!string.IsNullOrWhiteSpace(Oauth2ClientId))
                {
                    throw new ArgumentException("Not null", nameof(Oauth2ClientId));
                }
                if ((Oauth2ClientScopes?.Count ?? 0) > 0)
                {
                    throw new ArgumentException("Not null", nameof(Oauth2ClientScopes));
                }
                if (!string.IsNullOrWhiteSpace(Oauth2ClientSecret))
                {
                    throw new ArgumentException("Not null", nameof(Oauth2ClientSecret));
                }
                break;
            case ApiType.OAUTH2:
                _ = Guard.NotNullOrWhiteSpace(Oauth2BaseUri, nameof(Oauth2BaseUri));
                _ = Guard.NotNullOrWhiteSpace(Oauth2ClientId, nameof(Oauth2ClientId));
                _ = Guard.NotNullOrWhiteSpace(Oauth2ClientSecret, nameof(Oauth2ClientSecret));
                break;
            default:
                throw new ArgumentException($"Invalid ${ApiType}", nameof(ApiType));
        }
    }
}
