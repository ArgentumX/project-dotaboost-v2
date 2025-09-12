namespace Application.Common.Interfaces;

public interface IRequireSender
{
    Guid? SenderId { get; set; }
}