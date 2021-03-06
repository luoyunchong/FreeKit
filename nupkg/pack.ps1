. ".\common.ps1"


$PACKAGE_PATH = "packages"
if (!(Test-Path $PACKAGE_PATH)) {
    New-Item -type Directory -Path  $PACKAGE_PATH
}

# Delete old packages
Set-Location (Join-Path $rootFolder "nupkg/packages") 
Remove-Item "*.nupkg"
Remove-Item "*.snupkg"

# Rebuild all solutions
foreach ($solution in $solutions) {
    $solutionFolder = Join-Path $rootFolder $solution
    Set-Location $solutionFolder
    dotnet restore
}

# Create all packages
$i = 0
$projectsCount = $projects.length
Write-Info "Running dotnet pack on $projectsCount projects..."

foreach ($project in $projects) {
    $i += 1
    $projectFolder = Join-Path $rootFolder $project
    $projectName = ($project -split '/')[-1]
		
    # Create nuget pack
    Write-Info "[$i / $projectsCount] - Packing project: $projectName"

    # Delete 之前的版本信息
    $projectPackPath = Join-Path $projectFolder ("/bin/Release")
    Set-Location $projectPackPath
    Remove-Item "*.nupkg"
    Remove-Item "*.snupkg"

    Set-Location $projectFolder
    dotnet clean
    dotnet build --configuration Release

    if (-Not $?) {
        Write-Error "Packaging failed for the project: $projectName" 
        exit $LASTEXITCODE
    }
    
    # Move nuget package
    $projectName = $project.Substring($project.LastIndexOf("/") + 1)
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $projectName + ".*.nupkg")
    $projectPackPathSnupkg = Join-Path $projectFolder ("/bin/Release/" + $projectName + ".*.snupkg")
    $MovepackFolder = Join-Path $packFolder "/packages/"
    Move-Item -Force $projectPackPath $MovepackFolder
    Move-Item -Force $projectPackPathSnupkg $MovepackFolder
	

    Seperator
}

# Go back to the pack folder
Set-Location $packFolder