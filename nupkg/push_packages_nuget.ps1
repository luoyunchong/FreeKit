. ".\common.ps1"

$apiKey = $args[0]

if (!$apiKey) {	
    #reading password from file content
    $passwordFileName = "nuget-api-key.txt" 
    $pathExists = Test-Path -Path $passwordFileName -PathType Leaf
    if ($pathExists) {
        $apiKey = Get-Content $passwordFileName
        Write-Info "Using Nuget API Key from $passwordFileName ..." 
    }
}
 
if (!$apiKey) {
    $apiKey = Read-Host "Enter the Nuget API KEY"
}

# Get the version
[xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "src/Directory.Build.props")
$version = $commonPropsXml.Project.PropertyGroup[1].Version

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
    $nugetPackageExists = Test-Path $nugetPackageName -PathType leaf
 
    Write-Info "[$i / $totalProjectsCount] - Pushing: $nugetPackageName"
	
    if ($nugetPackageExists) {
        dotnet nuget push $nugetPackageName --skip-duplicate -s $nugetUrl --api-key "$apiKey"		
        #Write-Host ("Deleting package from local: " + $nugetPackageName)
        #Remove-Item $nugetPackageName -Force
    }
    else {
        Write-Host ("********** ERROR PACKAGE NOT FOUND: " + $nugetPackageName) -ForegroundColor red
        $errorCount += 1
        #Exit
    }
	
    Write-Host "--------------------------------------------------------------`r`n"
}

if ($errorCount > 0) {
    Write-Host ("******* $errorCount error(s) occured *******") -ForegroundColor red
}
