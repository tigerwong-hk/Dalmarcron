using AutoMapper.Configuration.Annotations;
using Dalmarcron.Scheduler.Core.Constants;
using Dalmarkit.Common.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dalmarcron.Scheduler.EntityFrameworkCore.Entities;

public class JobPublishedTransaction : ReadOnlyEntityBase
{
    [Key]
    [Required]
    [Ignore]
    public Guid JobPublishedTransactionId { get; set; }

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

    [Required]
    public string LambdaFunctionArn { get; set; } = null!;

    [Required]
    public string LambdaPermissionStatement { get; set; } = null!;

    [Required]
    public string LambdaTriggerArn { get; set; } = null!;

    public string? ApiHeaders { get; set; }

    public string? ApiIdempotencyKey { get; set; }

    public string? ApiJsonBody { get; set; }

    public string? Oauth2BaseUri { get; set; }

    public string? Oauth2ClientId { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? Oauth2ClientScopes { get; set; }

    public string? Oauth2ClientSecret { get; set; }

    #region Foreign Key
    [Required]
    public Guid ScheduledJobId { get; set; }
    #endregion Foreign Key

    #region Navigation
    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public ScheduledJob ScheduledJob { get; set; } = null!;
    #endregion Navigation
}
