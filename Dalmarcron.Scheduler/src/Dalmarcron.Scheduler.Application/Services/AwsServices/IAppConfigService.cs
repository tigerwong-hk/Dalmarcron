namespace Dalmarcron.Scheduler.Application.Services.AwsServices;

public interface IAppConfigService
{
    Task<IDictionary<string, string>> GetParameterAsync(string name);
    Task<IDictionary<string, string>> GetParametersByPathAsync(string path);
    Task SetSecretParameter(string name, string value);
    Task SetStringParameter(string name, string value);
    Task UpdateSecretParameter(string name, string value);
    Task UpdateStringParameter(string name, string value);
}
