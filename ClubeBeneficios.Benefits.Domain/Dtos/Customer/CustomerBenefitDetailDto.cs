namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public sealed class CustomerBenefitDetailDto : CustomerBenefitListItemDto
{
    public string? StackingRule { get; set; }
    public bool AllowFirstUseOnly { get; set; }
    public bool AutoActivateWhenApproved { get; set; }
}