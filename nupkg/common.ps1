# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

function Write-Info {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $text
    )

    Write-Host $text -ForegroundColor Black -BackgroundColor Green

    try {
        $host.UI.RawUI.WindowTitle = $text
    }		
    catch {
        #Changing window title is not suppoerted!
    }
}

function Write-Error {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $text
    )

    Write-Host $text -ForegroundColor Red -BackgroundColor Black 
}

function Seperator {
    Write-Host ("_" * 100)  -ForegroundColor gray 
}

function Get-Current-Version { 
    $commonPropsFilePath = resolve-path "../src/Directory.Build.props"
    $commonPropsXmlCurrent = [xml](Get-Content $commonPropsFilePath ) 
    $currentVersion = $commonPropsXmlCurrent.Project.PropertyGroup[1].Version.Trim()
    return $currentVersion
}

function Get-Current-Branch {
    return git branch --show-current
}	   

function Read-File {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $filePath
    )
		
    $pathExists = Test-Path -Path $filePath -PathType Leaf
    if ($pathExists) {
        return Get-Content $filePath		
    }
    else {
        Write-Error  "$filePath path does not exist!"
    }
}

# List of solutions
$solutions = (
    ""
)

# List of projects
$projects = (
    "src/IGeekFan.FreeKit.Modularity"

)