Add-Type -AssemblyName System.IO.Compression.FileSystem

$zipPath = 'C:\Users\radit\Downloads\LittleElsewhere_Unity_Project.zip'
$targetRoot = 'C:\Users\radit\LittleElsewhere'

Write-Host "Opening $zipPath ..."
$zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)

foreach ($entry in $zip.Entries) {
    if ([string]::IsNullOrWhiteSpace($entry.Name)) {
        # Directory entry
        continue
    }
    
    # entry.FullName starts with "LittleElsewhere/"
    $relPath = $entry.FullName
    if ($relPath.StartsWith("LittleElsewhere/")) {
        $relPath = $relPath.Substring("LittleElsewhere/".Length)
    }
    
    if ([string]::IsNullOrWhiteSpace($relPath)) { continue }

    $destPath = [System.IO.Path]::Combine($targetRoot, $relPath)
    $destDir = [System.IO.Path]::GetDirectoryName($destPath)

    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }

    [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destPath, $true)
    Write-Host "Extracted: $relPath"
}

$zip.Dispose()
Write-Host "Extraction complete!"
