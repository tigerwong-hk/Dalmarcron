namespace Dalmarcron.Scheduler.Application.Services.AwsServices;

public interface IScheduleService
{
    Task<string> CreateScheduleTriggerAsync(string triggerName, string scheduleExpression, string target, string description);
    Task DeleteScheduleTriggerAsync(string triggerName);
}
