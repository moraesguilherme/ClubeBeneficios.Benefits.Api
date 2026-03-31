param(
    [string]$ProjectRoot = (Get-Location).Path
)

$ErrorActionPreference = 'Stop'
$ProjectRoot = (Resolve-Path $ProjectRoot).Path

$repositoryPath = Join-Path $ProjectRoot 'ClubeBeneficios.Benefits.Infrastructure\Repositories\BenefitRepository.cs'
if (-not (Test-Path $repositoryPath)) {
    throw "Arquivo não encontrado: $repositoryPath"
}

$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$backupRoot = Join-Path $ProjectRoot "_backup_fix_openconnection_$timestamp"
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
Copy-Item $repositoryPath (Join-Path $backupRoot 'BenefitRepository.cs') -Force

$content = Get-Content $repositoryPath -Raw -Encoding UTF8

$old = 'private static async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)'
$new = 'private async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)'

if ($content -match [regex]::Escape($old)) {
    $content = $content.Replace($old, $new)
    Set-Content -Path $repositoryPath -Value $content -Encoding UTF8
    Write-Host "Correção aplicada com sucesso."
    Write-Host "Backup: $backupRoot"
    Write-Host "Alteração: OpenConnectionAsync deixou de ser static."
} else {
    Write-Host "Nenhuma alteração feita. O trecho esperado não foi encontrado."
    Write-Host "Verifique manualmente este método em:"
    Write-Host $repositoryPath
}
