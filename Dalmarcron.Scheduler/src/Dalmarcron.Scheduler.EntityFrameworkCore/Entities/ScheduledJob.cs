using AutoMapper.Configuration.Annotations;
using Dalmarcron.Scheduler.Core.Constants;
using Dalmarkit.Common.Entities.BaseEntities;
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

    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? ApiHeaders { get; set; }

    public string? ApiIdempotencyKey { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ApiJsonBody { get; set; }

    public string? Oauth2BaseUri { get; set; }

    public string? Oauth2ClientId { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? Oauth2ClientScopes { get; set; }

    public string? Oauth2ClientSecret { get; set; }
}
