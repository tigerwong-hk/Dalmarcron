using Dalmarkit.Cloud.Aws.Constants;
using Dalmarkit.Common.Validation;
using System.Text.RegularExpressions;

namespace Dalmarcron.Scheduler.Application.Options;

public class SchedulerOptions
{
    public string? LambdaArchitecture { get; set; }
    public string? LambdaDescription { get; set; }
    public string? LambdaFunctionNamePrefix { get; set; }
    public string? LambdaHandler { get; set; }
    public uint LambdaMemorySizeMb { get; set; }
    public string? LambdaRole { get; set; }
    public string? LambdaRuntime { get; set; }
    public string? LambdaS3Bucket { get; set; }
    public string? LambdaS3Key { get; set; }
    public int LambdaTimeoutSeconds { get; set; }
    public string? SsmParametersPathPrefix { get; set; }
    public string? SymmetricEncryptionSecretKey { get; set; }

    public string GetLambdaFunctionName(Guid scheduledJobId)
    {
        return $"{LambdaFunctionNamePrefix}-{scheduledJobId}";
    }

    public string GetLambdaTriggerName(Guid scheduledJobId)
    {
        return $"{LambdaFunctionNamePrefix}-{scheduledJobId}-trigger";
    }

    public void Validate()
    {
        _ = Guard.NotNullOrWhiteSpace(LambdaArchitecture, nameof(LambdaArchitecture));
        _ = Guard.NotNullOrWhiteSpace(LambdaDescription, nameof(LambdaDescription));
        _ = Guard.NotNullOrWhiteSpace(LambdaFunctionNamePrefix, nameof(LambdaFunctionNamePrefix));
        _ = Guard.NotNullOrWhiteSpace(LambdaHandler, nameof(LambdaHandler));
        _ = Guard.NotNullOrWhiteSpace(LambdaRole, nameof(LambdaRole));
        _ = Guard.NotNullOrWhiteSpace(LambdaRuntime, nameof(LambdaRuntime));
        _ = Guard.NotNullOrWhiteSpace(LambdaS3Bucket, nameof(LambdaS3Bucket));
        _ = Guard.NotNullOrWhiteSpace(LambdaS3Key, nameof(LambdaS3Key));
        _ = Guard.NotNullOrWhiteSpace(SsmParametersPathPrefix, nameof(SsmParametersPathPrefix));
        _ = Guard.NotNullOrWhiteSpace(SymmetricEncryptionSecretKey, nameof(SymmetricEncryptionSecretKey));

        if (!Regex.IsMatch(LambdaArchitecture!, Dalmarkit.Cloud.Aws.Constants.LambdaArchitecture.RegexPattern, RegexOptions.None, TimeSpan.FromMilliseconds(Dalmarkit.Cloud.Aws.Constants.LambdaArchitecture.RegexTimeoutIntervalMsec)))
        {
            throw new ArgumentException("Invalid", nameof(LambdaArchitecture));
        }

        if (LambdaHandler!.Length > Dalmarkit.Cloud.Aws.Constants.LambdaHandler.MaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(LambdaHandler), $"Length exceeds {Dalmarkit.Cloud.Aws.Constants.LambdaHandler.MaxLength}");
        }

        if (!Regex.IsMatch(LambdaHandler, Dalmarkit.Cloud.Aws.Constants.LambdaHandler.RegexPattern, RegexOptions.None, TimeSpan.FromMilliseconds(Dalmarkit.Cloud.Aws.Constants.LambdaHandler.RegexTimeoutIntervalMsec)))
        {
            throw new ArgumentException("Invalid", nameof(LambdaHandler));
        }

        if (LambdaMemorySizeMb is < Dalmarkit.Cloud.Aws.Constants.LambdaMemorySizeMb.Min or > Dalmarkit.Cloud.Aws.Constants.LambdaMemorySizeMb.Max)
        {
            throw new ArgumentOutOfRangeException(nameof(LambdaMemorySizeMb), $"Must be between {Dalmarkit.Cloud.Aws.Constants.LambdaMemorySizeMb.Min} and {Dalmarkit.Cloud.Aws.Constants.LambdaMemorySizeMb.Max} inclusive");
        }

        if (!Regex.IsMatch(LambdaRuntime!, Dalmarkit.Cloud.Aws.Constants.LambdaRuntime.RegexPattern, RegexOptions.None, TimeSpan.FromMilliseconds(Dalmarkit.Cloud.Aws.Constants.LambdaRuntime.RegexTimeoutIntervalMsec)))
        {
            throw new ArgumentException("Invalid", nameof(LambdaRuntime));
        }

        if (LambdaS3Bucket!.Length is < S3Bucket.MinLength or > S3Bucket.MaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(LambdaS3Bucket), $"Must be between {S3Bucket.MinLength} and {S3Bucket.MaxLength} inclusive");
        }

        if (!Regex.IsMatch(LambdaS3Bucket, S3Bucket.RegexPattern, RegexOptions.None, TimeSpan.FromMilliseconds(S3Bucket.RegexTimeoutIntervalMsec)))
        {
            throw new ArgumentException("Invalid", nameof(LambdaS3Bucket));
        }

        if (LambdaS3Key!.Length > S3Key.MaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(LambdaS3Key), $"Length exceeds {S3Key.MaxLength}");
        }

        if (LambdaTimeoutSeconds is < Dalmarkit.Cloud.Aws.Constants.LambdaTimeoutSeconds.Min or > Dalmarkit.Cloud.Aws.Constants.LambdaTimeoutSeconds.Max)
        {
            throw new ArgumentOutOfRangeException(nameof(LambdaTimeoutSeconds), $"Must be between {Dalmarkit.Cloud.Aws.Constants.LambdaTimeoutSeconds.Min} and {Dalmarkit.Cloud.Aws.Constants.LambdaTimeoutSeconds.Max} inclusive");
        }
    }
}
