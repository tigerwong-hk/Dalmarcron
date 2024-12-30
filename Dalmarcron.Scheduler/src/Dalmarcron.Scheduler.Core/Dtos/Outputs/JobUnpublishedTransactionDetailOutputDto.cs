namespace Dalmarcron.Scheduler.Core.Dtos.Outputs;

public class JobUnpublishedTransactionDetailOutputDto : JobUnpublishedTransactionOutputDto
{
    public string? ApiIdempotencyKey { get; set; }

    public string? Oauth2BaseUri { get; set; }

    public IEnumerable<string>? Oauth2ClientScopes { get; set; }

    public string CreatorId { get; set; } = null!;
}
