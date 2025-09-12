using AutoMapper;
using Domain.Common;

namespace Application.Common.Dtos;

public abstract class BaseEntityDto
{
    public Guid Id { get; init; }
}