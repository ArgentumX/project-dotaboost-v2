using System.Reflection;
using Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.UnitTests;

public static class ServiceFabric
{
    public static  IMapper Mapper = CreateMapper(Assembly.GetAssembly(typeof(IApplicationDbContext))!);
    private static IMapper CreateMapper(params Assembly[] assembliesWithProfiles)
    {
        var loggerFactory = LoggerFactory.Create(cfg => { });
        var configuration = new MapperConfiguration(
            cfg => cfg.AddMaps(assembliesWithProfiles),
            loggerFactory
        );

        var mapper = configuration.CreateMapper();
        return mapper;
    }
}