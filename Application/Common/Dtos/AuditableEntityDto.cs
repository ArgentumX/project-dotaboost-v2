using AutoMapper;
using Domain.Common;

namespace Application.Common.Dtos;

public abstract class AuditableEntityDto : BaseEntityDto
{
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastModified { get; init; }
}