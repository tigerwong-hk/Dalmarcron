using Dalmarcron.Scheduler.Core.Constants;

namespace Dalmarcron.Scheduler.Core.Dtos.Outputs;

public class JobPublishedTransactionOutputDto
{
    public Guid JobPublishedTransactionId { get; set; }

    public ApiMethod ApiMethod { get; set; }

    public ApiType ApiType { get; set; }

    public string CronExpression { get; set; } = null!;

    public string JobName { get; set; } = null!;

    public string LambdaFunctionArn { get; set; } = null!;

    public string LambdaPermissionStatement { get; set; } = null!;

    public string LambdaTriggerArn { get; set; } = null!;

    public Guid ScheduledJobId { get; set; }

    public DateTime CreatedOn { get; set; }
}
