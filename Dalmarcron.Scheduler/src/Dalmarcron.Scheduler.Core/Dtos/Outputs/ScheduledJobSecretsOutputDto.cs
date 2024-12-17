namespace Dalmarcron.Scheduler.Core.Dtos.Outputs;

public class ScheduledJobSecretsOutputDto : ScheduledJobDetailOutputDto
{
    public string ApiUrl { get; set; } = null!;

    public IDictionary<string, string>? ApiHeaders { get; set; }

    public string? ApiJsonBody { get; set; }

    public string? Oauth2ClientId { get; set; }

    public string? Oauth2ClientSecret { get; set; }
}
