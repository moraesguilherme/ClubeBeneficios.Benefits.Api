namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLevelScopeDto
{
    public string? LevelType { get; set; }

    // Campo esperado hoje pelo BenefitContractMapper
    public string? LevelCode { get; set; }

    // Compatibilidade adicional caso outras partes do cÃ³digo usem lista
    public IReadOnlyCollection<string> LevelCodes =>
        string.IsNullOrWhiteSpace(LevelCode)
            ? Array.Empty<string>()
            : LevelCode!
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
