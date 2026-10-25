$csprojPath = 'Assembly-CSharp.csproj'
[xml]$xml = Get-Content $csprojPath

# Remove Analyzers pointing to Library\PackageCache
$analyzersToRemove = @()
foreach ($itemGroup in $xml.Project.ItemGroup) {
    if ($itemGroup.Analyzer) {
        foreach ($analyzer in $itemGroup.Analyzer) {
            if ($analyzer.Include -like "*Library\PackageCache*") {
                $analyzersToRemove += $analyzer
            }
        }
    }
}

foreach ($analyzer in $analyzersToRemove) {
    $parent = $analyzer.ParentNode
    $parent.RemoveChild($analyzer) | Out-Null
}

# Save back
$xml.Save((Get-Item $csprojPath).FullName)
Write-Host "Cleaned PackageCache analyzers from Assembly-CSharp.csproj"
