using FluentValidation;

namespace Dalmarcron.Scheduler.Core.Validators;

public class DefaultValidator<T> : AbstractValidator<T>
{
    public DefaultValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}
