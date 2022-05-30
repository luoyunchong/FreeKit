. ".\common.ps1" 

# Build all solutions   

Write-Host $solutions

foreach ($solution in $solutions) {    
    $solutionAbsPath = (Join-Path $rootFolder $solution)
    Set-Location $solutionAbsPath
    dotnet build
    if (-Not $?) {
        Write-Host ("Build failed for the solution: " + $solution)
        Set-Location $rootFolder
        exit $LASTEXITCODE
    }
}

Set-Location $packFolder 
