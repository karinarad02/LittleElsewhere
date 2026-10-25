$csproj = Get-Content 'Assembly-CSharp.csproj' -Raw
$newCompile = '    <Compile Include="Assets\Scripts\Runtime\*.cs" />'

if ($csproj -notmatch 'Assets\\Scripts\\Runtime') {
    $csproj = $csproj -replace '(?=</ItemGroup>\s*<ItemGroup>\s*<Reference)', "$newCompile`r`n  "
    Set-Content -Path 'Assembly-CSharp.csproj' -Value $csproj
    Write-Host "Updated Assembly-CSharp.csproj with Runtime scripts"
} else {
    Write-Host "Assembly-CSharp.csproj already includes Runtime scripts"
}
