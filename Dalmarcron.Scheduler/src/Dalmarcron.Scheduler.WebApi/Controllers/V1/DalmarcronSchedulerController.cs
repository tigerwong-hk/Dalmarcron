using Dalmarcron.Scheduler.Application.Services.ApplicationServices;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarkit.AspNetCore.Controllers;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Identity;
using Dalmarkit.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dalmarcron.Scheduler.WebApi.Controllers.V1;

[Authorize]
public class DalmarcronSchedulerController(IDalmarcronSchedulerQueryService dalmarCronSchedulerQueryService,
    IDalmarcronSchedulerCommandService dalmarcronSchedulerCommandService) : RestApiControllerBase
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = {
            new JsonStringEnumConverter()
        }
    };

    private readonly IDalmarcronSchedulerCommandService _dalmarcronSchedulerCommandService = Guard.NotNull(dalmarcronSchedulerCommandService, nameof(dalmarcronSchedulerCommandService));
    private readonly IDalmarcronSchedulerQueryService _dalmarcronSchedulerQueryService = Guard.NotNull(dalmarCronSchedulerQueryService, nameof(dalmarCronSchedulerQueryService));

    #region ScheduledJob
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpPost]
    public async Task<ActionResult> CreateScheduledJobAsync([FromBody] CreateScheduledJobInputDto inputDto)
    {
        AuditDetail auditDetail = CreateAuditDetail();
        Result<Guid, ErrorDetail> result = await _dalmarcronSchedulerCommandService.CreateScheduledJobAsync(inputDto, auditDetail);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpDelete]
    public async Task<ActionResult> DeleteScheduledJobAsync([FromBody] DeleteScheduledJobInputDto inputDto)
    {
        AuditDetail auditDetail = CreateAuditDetail();
        Result<Guid, ErrorDetail> result = await _dalmarcronSchedulerCommandService.DeleteScheduledJobAsync(inputDto, auditDetail);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpGet]
    public async Task<ActionResult> GetPublishedJobDetailAsync([FromQuery] GetPublishedJobDetailInputDto inputDto)
    {
        Result<PublishedJobDetailOutputDto, ErrorDetail> result = await _dalmarcronSchedulerQueryService.GetPublishedJobDetailAsync(inputDto);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpGet]
    public async Task<ActionResult> GetScheduledJobDetailAsync([FromQuery] GetScheduledJobDetailInputDto inputDto)
    {
        Result<ScheduledJobDetailOutputDto, ErrorDetail> result = await _dalmarcronSchedulerQueryService.GetScheduledJobDetailAsync(inputDto);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpGet]
    public async Task<ActionResult> GetScheduledJobListAsync([FromQuery] GetScheduledJobListInputDto inputDto)
    {
        Result<ResponsePagination<ScheduledJobOutputDto>, ErrorDetail> result = await _dalmarcronSchedulerQueryService.GetScheduledJobListAsync(inputDto);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpGet]
    public async Task<ActionResult> GetScheduledJobSecretsAsync([FromQuery] GetScheduledJobSecretsInputDto inputDto)
    {
        Result<ScheduledJobSecretsOutputDto, ErrorDetail> result = await _dalmarcronSchedulerQueryService.GetScheduledJobSecretsAsync(inputDto);

        return ApiResponse(result, _jsonSerializerOptions);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpDelete]
    public async Task<ActionResult> PublishScheduledJobAsync([FromBody] PublishScheduledJobInputDto inputDto)
    {
        AuditDetail auditDetail = CreateAuditDetail();
        Result<Guid, ErrorDetail> result = await _dalmarcronSchedulerCommandService.PublishScheduledJobAsync(inputDto, auditDetail);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpDelete]
    public async Task<ActionResult> UnpublishScheduledJobAsync([FromBody] UnpublishScheduledJobInputDto inputDto)
    {
        AuditDetail auditDetail = CreateAuditDetail();
        Result<Guid, ErrorDetail> result = await _dalmarcronSchedulerCommandService.UnpublishScheduledJobAsync(inputDto, auditDetail);

        return ApiResponse(result);
    }

    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
    [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
    [HttpPut]
    public async Task<ActionResult> UpdateScheduledJobAsync([FromBody] UpdateScheduledJobInputDto inputDto)
    {
        AuditDetail auditDetail = CreateAuditDetail();
        Result<Guid, ErrorDetail> result = await _dalmarcronSchedulerCommandService.UpdateScheduledJobAsync(inputDto, auditDetail);

        return ApiResponse(result);
    }
    #endregion ScheduledJob
}
