using System.Reflection;
using System.Runtime.CompilerServices;
using Application.Batches;
using Application.BoosterApplications;
using Application.Boosters;
using Application.BoostOrders;
using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        var loggerFactory = LoggerFactory.Create(cfg => {});
        _configuration = new MapperConfiguration(
            cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))),
            loggerFactory
        );

        _mapper = _configuration.CreateMapper();
    }
    
    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }
    
    [Test]
    [TestCase(typeof(Booster), typeof(BoosterDto))]
    [TestCase(typeof(BoostOrder), typeof(BoostOrderDto))]
    [TestCase(typeof(Batch), typeof(BatchDto))]
    [TestCase(typeof(BoosterApplication), typeof(BoosterApplicationDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;
        
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}