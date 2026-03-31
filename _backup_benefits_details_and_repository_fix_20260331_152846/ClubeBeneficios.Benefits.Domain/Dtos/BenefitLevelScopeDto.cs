using System;

namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLevelScopeDto
{
    public Guid? Id { get; set; }
    public string? LevelType { get; set; }
    public string? LevelCode { get; set; }
}