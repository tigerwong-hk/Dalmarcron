using AutoMapper;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Cloud.Aws.Mappers;
using Dalmarkit.Common.Cryptography;
using Dalmarkit.Common.Services;
using Dalmarkit.EntityFrameworkCore.Mappers;
using System.Text.Json;

namespace Dalmarcron.Scheduler.Application.Mappers;

public class MapperConfigurations : MapperConfigurationBase
{
    protected override void DtoToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
        FunctionConfigMapper.CreateMap(config);
    }

    protected override void DtoToEntityMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<CreateScheduledJobInputDto, ScheduledJob>()
            .ForMember(d => d.ApiUrl, opt => opt.MapFrom((src, _, _, context) =>
                AuthenticatedEncryption.Encrypt(
                    src.ApiUrl,
                    (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                )
            ))
            .ForMember(d => d.ApiHeaders, opt => opt.MapFrom((src, _, _, context) =>
                (src.ApiHeaders?.Count ?? 0) > 0
                    ? AuthenticatedEncryption.Encrypt(
                        JsonSerializer.Serialize(src.ApiHeaders),
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
                    : null
            ))
            .ForMember(d => d.ApiJsonBody, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.ApiJsonBody)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.ApiJsonBody,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientId, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientId)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.Oauth2ClientId,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientSecret, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientSecret)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.Oauth2ClientSecret,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ));

        _ = config.CreateMap<UpdateScheduledJobInputDto, ScheduledJob>()
            .ForMember(d => d.ApiUrl, opt => opt.MapFrom((src, _, _, context) =>
                AuthenticatedEncryption.Encrypt(
                    src.ApiUrl,
                    (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                )
            ))
            .ForMember(d => d.ApiHeaders, opt => opt.MapFrom((src, _, _, context) =>
                (src.ApiHeaders?.Count ?? 0) > 0
                    ? AuthenticatedEncryption.Encrypt(
                        JsonSerializer.Serialize(src.ApiHeaders),
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
                    : null
            ))
            .ForMember(d => d.ApiJsonBody, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.ApiJsonBody)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.ApiJsonBody,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientId, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientId)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.Oauth2ClientId,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientSecret, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientSecret)
                    ? null
                    : AuthenticatedEncryption.Encrypt(
                        src.Oauth2ClientSecret,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ));

        _ = config.CreateMap<ScheduledJob, JobPublishedTransaction>()
            .ForMember(d => d.CreateRequestId, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MapperItemKeys.CreateRequestId]
            ))
            .ForMember(d => d.LambdaFunctionArn, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MapperItemKeys.LambdaFunctionArn]
            ))
            .ForMember(d => d.LambdaPermissionStatement, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MapperItemKeys.LambdaPermissionStatement]
            ))
            .ForMember(d => d.LambdaTriggerArn, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MapperItemKeys.LambdaTriggerArn]
            ));

        _ = config.CreateMap<ScheduledJob, JobUnpublishedTransaction>()
            .ForMember(d => d.CreateRequestId, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MapperItemKeys.CreateRequestId]
            ));
    }

    protected override void EntityToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<JobPublishedTransaction, JobPublishedTransactionOutputDto>();
        _ = config.CreateMap<JobPublishedTransaction, JobPublishedTransactionDetailOutputDto>();

        _ = config.CreateMap<JobUnpublishedTransaction, JobUnpublishedTransactionOutputDto>();
        _ = config.CreateMap<JobUnpublishedTransaction, JobUnpublishedTransactionDetailOutputDto>();

        _ = config.CreateMap<ScheduledJob, PublishedJobDetailOutputDto>()
            .ForMember(d => d.Function, opt => opt.MapFrom((_, _, _, context) =>
                (FunctionConfig)context.Items[MapperItemKeys.FunctionConfig]
            ));

        _ = config.CreateMap<ScheduledJob, ScheduledJobOutputDto>();
        _ = config.CreateMap<ScheduledJob, ScheduledJobDetailOutputDto>();

        _ = config.CreateMap<ScheduledJob, ScheduledJobSecretsOutputDto>()
            .ForMember(d => d.ApiUrl, opt => opt.MapFrom((src, _, _, context) =>
                AuthenticatedEncryption.Decrypt(
                    src.ApiUrl,
                    (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                )
            ))
            .ForMember(d => d.ApiHeaders, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.ApiHeaders)
                    ? null
                    : JsonSerializer.Deserialize<IDictionary<string, string>>(
                        AuthenticatedEncryption.Decrypt(
                            src.ApiHeaders,
                            (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                        )
                    )
            ))
            .ForMember(d => d.ApiJsonBody, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.ApiJsonBody)
                    ? null
                    : AuthenticatedEncryption.Decrypt(
                        src.ApiJsonBody,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientId, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientId)
                    ? null
                    : AuthenticatedEncryption.Decrypt(
                        src.Oauth2ClientId,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ))
            .ForMember(d => d.Oauth2ClientSecret, opt => opt.MapFrom((src, _, _, context) =>
                string.IsNullOrWhiteSpace(src.Oauth2ClientSecret)
                    ? null
                    : AuthenticatedEncryption.Decrypt(
                        src.Oauth2ClientSecret,
                        (string)context.Items[MapperItemKeys.SymmetricEncryptionSecretKey]
                    )
            ));
    }
}
