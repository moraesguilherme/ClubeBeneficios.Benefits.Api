namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public sealed class CreateCustomerBenefitRequestDto
{
    public Guid? PetId { get; set; }
    public DateTime? ScheduledFor { get; set; }
}