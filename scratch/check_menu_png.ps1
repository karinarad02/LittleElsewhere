Add-Type -AssemblyName System.Drawing
$filePath = 'C:\Users\radit\Downloads\menus.png'
if (Test-Path $filePath) {
    $img = [System.Drawing.Image]::FromFile($filePath)
    Write-Host "File exists!"
    Write-Host "Width: $($img.Width)"
    Write-Host "Height: $($img.Height)"
    Write-Host "PixelFormat: $($img.PixelFormat)"
    $img.Dispose()
} else {
    Write-Host "File not found"
}
