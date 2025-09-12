namespace Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message = "Bad request") : base(message)
    {
    }
}