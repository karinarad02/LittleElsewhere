Add-Type -AssemblyName System.Drawing
$filePath = 'C:\Users\radit\Downloads\menus.png'
$img = [System.Drawing.Bitmap]::FromFile($filePath)

Write-Host "Sampling 1536x1024 image..."
$w = $img.Width
$h = $img.Height

# Save quarter previews or quadrants if it's a sprite sheet / UI layout
$dstDir = 'C:\Users\radit\LittleElsewhere\Assets\Resources\Menu_Components'
if (-not (Test-Path $dstDir)) { New-Item -ItemType Directory -Path $dstDir }

# Let's inspect center and grid points
Write-Host "Top-Left (100,100): $($img.GetPixel(100,100))"
Write-Host "Center (768,512): $($img.GetPixel(768,512))"
Write-Host "Bottom-Right (1400,900): $($img.GetPixel(1400,900))"

$img.Dispose()
