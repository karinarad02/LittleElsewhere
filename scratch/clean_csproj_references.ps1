$csprojPath = 'Assembly-CSharp.csproj'
[xml]$xml = Get-Content $csprojPath

$refsToRemove = @()
foreach ($itemGroup in $xml.Project.ItemGroup) {
    if ($itemGroup.Reference) {
        foreach ($ref in $itemGroup.Reference) {
            if ($ref.HintPath) {
                $path = $ref.HintPath
                if (-not (Test-Path $path)) {
                    $refsToRemove += $ref
                }
            }
        }
    }
}

foreach ($ref in $refsToRemove) {
    $parent = $ref.ParentNode
    $parent.RemoveChild($ref) | Out-Null
}

$xml.Save((Get-Item $csprojPath).FullName)
Write-Host "Removed $($refsToRemove.Count) non-existent reference tags from Assembly-CSharp.csproj"
