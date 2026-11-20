param(
    [string]$OutputPath = (Join-Path $PSScriptRoot 'packages')
)

$ErrorActionPreference = 'Stop'
$solution = Join-Path $PSScriptRoot '../FreeKit-Core.slnx'
$outputDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputPath)

dotnet pack $solution --configuration Release "-p:PackageOutputPath=$outputDirectory"
if ($LASTEXITCODE -ne 0) {
    throw "FreeKit packaging failed with exit code $LASTEXITCODE."
}
