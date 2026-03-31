namespace ClubeBeneficios.Benefits.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base("not_found", message)
    {
    }
}