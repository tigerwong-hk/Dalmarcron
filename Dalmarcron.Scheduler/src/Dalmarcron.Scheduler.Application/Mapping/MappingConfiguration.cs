using AutoMapper;
using Dalmarcron.Scheduler.Core.Dtos.Inputs;
using Dalmarcron.Scheduler.Core.Dtos.Outputs;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.EntityFrameworkCore.Mappers;

namespace Dalmarcron.Scheduler.Application.Mapping;

public class MapperConfigurations : MapperConfigurationBase
{
    protected override void DtoToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
    }

    protected override void DtoToEntityMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<CreateScheduledJobInputDto, ScheduledJob>();
        _ = config.CreateMap<UpdateScheduledJobInputDto, ScheduledJob>();
    }

    protected override void EntityToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<ScheduledJob, ScheduledJobOutputDto>();
        _ = config.CreateMap<ScheduledJob, ScheduledJobDetailOutputDto>();
    }
}
