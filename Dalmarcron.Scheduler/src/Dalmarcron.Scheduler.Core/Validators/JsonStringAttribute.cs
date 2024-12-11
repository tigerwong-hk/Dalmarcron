using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Dalmarcron.Scheduler.Core.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class JsonStringAttribute : ValidationAttribute
{
    public JsonStringAttribute() : base(ErrorMessages.ModelStateErrors.ValueInvalid)
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is not string valueAsString)
        {
            return false;
        }

        try
        {
            _ = JsonDocument.Parse(valueAsString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
