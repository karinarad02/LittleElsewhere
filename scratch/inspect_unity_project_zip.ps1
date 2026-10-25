Add-Type -AssemblyName System.IO.Compression.FileSystem

$zipPath = 'C:\Users\radit\Downloads\LittleElsewhere_Unity_Project.zip'
if (Test-Path $zipPath) {
    $zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
    Write-Host "Total entries: $($zip.Entries.Count)"
    $zip.Entries | Select-Object -First 100 -Property FullName, Length
    $zip.Dispose()
} else {
    Write-Host "Zip file not found at $zipPath"
}
