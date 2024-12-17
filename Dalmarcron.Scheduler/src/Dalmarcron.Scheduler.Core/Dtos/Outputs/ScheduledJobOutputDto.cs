using Dalmarcron.Scheduler.Core.Constants;

namespace Dalmarcron.Scheduler.Core.Dtos.Outputs;

public class ScheduledJobOutputDto
{
    public Guid ScheduledJobId { get; set; }

    public ApiMethod ApiMethod { get; set; }

    public ApiType ApiType { get; set; }

    public string CronExpression { get; set; } = null!;

    public string JobName { get; set; } = null!;

    public PublicationState PublicationState { get; set; }

    public DateTime CreatedOn { get; set; }
}
