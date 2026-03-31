namespace ClubeBeneficios.Benefits.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base("forbidden", message)
    {
    }
}