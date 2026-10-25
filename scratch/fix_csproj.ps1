$csprojPath = 'Assembly-CSharp.csproj'
$csproj = Get-Content $csprojPath -Raw

# Replace ItemGroup containing Compile with new entries
$runtimeScripts = Get-ChildItem -Path Assets\Scripts\Runtime -Filter *.cs | ForEach-Object { "    <Compile Include=""Assets\Scripts\Runtime\$($_.Name)"" />" }
$editorScripts = Get-ChildItem -Path Assets\Scripts\Editor -Filter *.cs | ForEach-Object { "    <Compile Include=""Assets\Scripts\Editor\$($_.Name)"" />" }

$compileBlock = "  <ItemGroup>`r`n" + ($runtimeScripts -join "`r`n") + "`r`n" + ($editorScripts -join "`r`n") + "`r`n  </ItemGroup>"

# Regex replace existing Compile ItemGroup
$csproj = $csproj -replace '(?s)\s*<ItemGroup>\s*<Compile Include="Assets\\Scripts.*?</ItemGroup>', "`r`n$compileBlock"

Set-Content -Path $csprojPath -Value $csproj -Encoding UTF8
Write-Host "Updated Assembly-CSharp.csproj successfully"
