. ".\common.ps1"

[xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "src/Directory.Build.props")
$version = $commonPropsXml.Project.PropertyGroup[1].Version

Write-Info "Committing changes to GitHub"

cd ..
git add .
git commit -m v$version
git push 

git tag v$version
git push --tags

Write-Info "Completed: Committing changes to GitHub"

cd nupkg