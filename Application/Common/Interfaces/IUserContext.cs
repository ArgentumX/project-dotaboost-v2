namespace Application.Common.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    string? UserEmail { get; }
    bool IsInRole(string role);
}