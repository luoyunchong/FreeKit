. ".\common.ps1"

$apiKey = $args[0]
$version = $args[1]

if (!$apiKey) {	
    #reading password from file content
    $passwordFileName = "nuget-api-key-delete.txt" 
    $pathExists = Test-Path -Path $passwordFileName -PathType Leaf
    if ($pathExists) {
        $apiKey = Get-Content $passwordFileName
        Write-Info "Using Nuget API Key from $passwordFileName ..." 
    }
}
 
if (!$apiKey) {
    $apiKey = Read-Host "Enter the Nuget API KEY"
}
if (!$version) {
    # Get the version
    [xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "src/Directory.Build.props")
    $version = $commonPropsXml.Project.PropertyGroup[1].Version
}

# Publish all packages
$i = 0
$errorCount = 0
$totalProjectsCount = $projects.length
$nugetUrl = "https://api.nuget.org/v3/index.json"
$MovepackFolder = Join-Path $packFolder "/packages/"
Set-Location $MovepackFolder
foreach ($project in $projects) {
    $i += 1
    $projectName = ($project -split '/')[-1]
    $nugetPackageName = $projectName + "." + $version.Trim() + ".nupkg"	
 
    Write-Info "[$i / $totalProjectsCount] - Deleting: $nugetPackageName"
    #dotnet nuget delete IGeekFan.Localization.FreeSql 0.0.3 --non-interactive -k oy2jxmgb4k3hmg65v7vauqjldawwh5gefbxgg2sdwpkmfy -s https://api.nuget.org/v3/index.json
    dotnet nuget delete $projectName $version --non-interactive -s $nugetUrl -k "$apiKey"		
    Write-Host ("Deleting package from local: " + $nugetPackageName)
    #Remove-Item $nugetPackageName -Force
  
	
    Write-Host "--------------------------------------------------------------`r`n"
}

if ($errorCount > 0) {
    Write-Host ("******* $errorCount error(s) occured *******") -ForegroundColor red
}
Set-Location $packFolder