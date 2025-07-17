using Dalmarkit.Cloud.Aws.Dtos.Outputs;

namespace Dalmarcron.Scheduler.Core.Dtos.Outputs;

public class PublishedJobDetailOutputDto : ScheduledJobDetailOutputDto
{
    public FunctionConfigOutputDto Function { get; set; } = null!;
}
