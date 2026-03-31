param(
    [string]$ProjectRoot = (Get-Location).Path,
    [switch]$RestoreBenefitRepositoryFromGit = $true
)

$ErrorActionPreference = 'Stop'
$ProjectRoot = (Resolve-Path $ProjectRoot).Path

function Backup-File {
    param([string]$FilePath, [string]$BackupRoot)
    if (-not (Test-Path $FilePath)) { return }
    $rootResolved = (Resolve-Path $ProjectRoot).Path
    $fileResolved = (Resolve-Path $FilePath).Path
    $relative = $fileResolved.Substring($rootResolved.Length).TrimStart('\','/')
    $backupPath = Join-Path $BackupRoot $relative
    $backupDir = Split-Path $backupPath -Parent
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    }
    Copy-Item $fileResolved $backupPath -Force
}

$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$backupRoot = Join-Path $ProjectRoot "_backup_fix_benefits_dtos_mapper_$timestamp"
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null

$dtosDir = Join-Path $ProjectRoot 'ClubeBeneficios.Benefits.Domain\Dtos'
$levelScopePath = Join-Path $dtosDir 'BenefitLevelScopeDto.cs'
$behaviorRulesPath = Join-Path $dtosDir 'BenefitBehaviorRulesDto.cs'
$codeRulesPath = Join-Path $dtosDir 'BenefitCodeRulesDto.cs'
$detailsPath = Join-Path $dtosDir 'BenefitDetailsDto.cs'
$repositoryPath = Join-Path $ProjectRoot 'ClubeBeneficios.Benefits.Infrastructure\Repositories\BenefitRepository.cs'

foreach ($path in @($levelScopePath, $behaviorRulesPath, $codeRulesPath, $detailsPath, $repositoryPath)) {
    if (Test-Path $path) {
        Backup-File -FilePath $path -BackupRoot $backupRoot
    }
}

# DTO 1 - exatamente no formato esperado pelo BenefitContractMapper
$levelScopeContent = @'
namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLevelScopeDto
{
    public string? LevelType { get; set; }

    // Campo esperado hoje pelo BenefitContractMapper
    public string? LevelCode { get; set; }

    // Compatibilidade adicional caso outras partes do código usem lista
    public IReadOnlyCollection<string> LevelCodes =>
        string.IsNullOrWhiteSpace(LevelCode)
            ? Array.Empty<string>()
            : LevelCode!
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
'@

# DTO 2 - propriedades exatas esperadas pelo mapper
$behaviorRulesContent = @'
namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitBehaviorRulesDto
{
    public bool MinFrequencyEnabled { get; set; }
    public int? MinFrequencyValue { get; set; }
    public int? FrequencyWindowMonths { get; set; }

    public bool MinTicketEnabled { get; set; }
    public decimal? MinTicketValue { get; set; }
    public int? TicketWindowMonths { get; set; }

    public bool FirstUseOnly { get; set; }
    public bool RequiresMatilhaApproval { get; set; }

    public string? CustomRuleText { get; set; }
}
'@

# DTO 3 - propriedade exata esperada pelo mapper
$codeRulesContent = @'
namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitCodeRulesDto
{
    public bool RequiresAccessCode { get; set; }
    public bool AllowAnyActivePartnerCode { get; set; }
    public Guid? SpecificAccessCodeId { get; set; }

    // Campo esperado hoje pelo BenefitContractMapper
    public string? CodeValidationMode { get; set; }
}
'@

Set-Content -Path $levelScopePath -Value $levelScopeContent -Encoding UTF8
Set-Content -Path $behaviorRulesPath -Value $behaviorRulesContent -Encoding UTF8
Set-Content -Path $codeRulesPath -Value $codeRulesContent -Encoding UTF8

# Corrige BenefitDetailsDto apenas se estiver sem using LINQ
if (Test-Path $detailsPath) {
    $detailsContent = Get-Content $detailsPath -Raw -Encoding UTF8
    if ($detailsContent -notmatch 'using\s+System\.Linq;') {
        $detailsContent = "using System.Linq;`r`n" + $detailsContent.TrimStart([char]0xFEFF)
        Set-Content -Path $detailsPath -Value $detailsContent -Encoding UTF8
    }
}

# O repo público atual usa IDbConnection injetado, não _connectionString.
# Se o arquivo local foi corrompido pelos scripts anteriores, o caminho mais seguro é restaurar do git HEAD.
if ($RestoreBenefitRepositoryFromGit -and (Test-Path $repositoryPath)) {
    Push-Location $ProjectRoot
    try {
        $gitAvailable = Get-Command git -ErrorAction SilentlyContinue
        if ($null -eq $gitAvailable) {
            Write-Warning "Git não encontrado no PATH. Pulei a restauração do BenefitRepository.cs."
        }
        else {
            git checkout -- "ClubeBeneficios.Benefits.Infrastructure/Repositories/BenefitRepository.cs" | Out-Null
        }
    }
    finally {
        Pop-Location
    }
}

Write-Host "Correção aplicada com sucesso."
Write-Host "Backup criado em: $backupRoot"
Write-Host ""
Write-Host "Arquivos criados/ajustados:"
Write-Host " - ClubeBeneficios.Benefits.Domain/Dtos/BenefitLevelScopeDto.cs"
Write-Host " - ClubeBeneficios.Benefits.Domain/Dtos/BenefitBehaviorRulesDto.cs"
Write-Host " - ClubeBeneficios.Benefits.Domain/Dtos/BenefitCodeRulesDto.cs"
Write-Host " - ClubeBeneficios.Benefits.Domain/Dtos/BenefitDetailsDto.cs (apenas using, se necessário)"
if ($RestoreBenefitRepositoryFromGit) {
    Write-Host " - ClubeBeneficios.Benefits.Infrastructure/Repositories/BenefitRepository.cs (restaurado do git HEAD)"
}
Write-Host ""
Write-Host "Próximo passo:"
Write-Host "  1) dotnet clean"
Write-Host "  2) dotnet build"
