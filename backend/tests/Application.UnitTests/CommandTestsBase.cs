using System.Reflection;
using Application.Common.Interfaces;
using AutoMapper;
using Moq;

namespace Application.UnitTests;

public abstract class CommandTestsBase
{
    protected IMapper RealMapper { get; private set; }
    
    [OneTimeSetUp]
    public virtual void OneTimeInit()
    {
        RealMapper = ServiceFabric.Mapper;
    }
    
}