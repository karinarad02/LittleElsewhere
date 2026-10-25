# Remove old non-zip script folders from Assets/Scripts
$oldDirs = @(
    'Assets/Scripts/Core',
    'Assets/Scripts/Saving',
    'Assets/Scripts/Stories',
    'Assets/Scripts/Time',
    'Assets/Scripts/UI',
    'Assets/Scripts/World',
    'Assets/Scripts/Environment',
    'Assets/Scripts/Lighting',
    'Assets/Scripts/Navigation',
    'Assets/Scripts/Player',
    'Assets/Tests'
)

foreach ($dir in $oldDirs) {
    if (Test-Path $dir) {
        Remove-Item -Path $dir -Recurse -Force
        Write-Host "Removed old directory: $dir"
    }
}

$oldFiles = @(
    'Assets/Scripts/LEGame.cs',
    'Assets/Scripts/LEPlayer.cs',
    'Assets/Scripts/LECamera.cs',
    'Assets/Scripts/LEAnimal.cs'
)

foreach ($file in $oldFiles) {
    if (Test-Path $file) {
        Remove-Item -Path $file -Force
        Write-Host "Removed old file: $file"
    }
}
