Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipPath = 'C:\Users\radit\Downloads\LittleElsewhere_Unity_Project.zip'
$zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
$zip.Entries | ForEach-Object { $_.FullName } | Out-File -FilePath 'scratch/zip_entries.txt' -Encoding utf8
$zip.Dispose()
