Add-Type -AssemblyName System.Drawing

$srcPath = 'C:\Users\radit\Downloads\menus.png'
$bmp = [System.Drawing.Bitmap]::FromFile($srcPath)
$w = $bmp.Width
$h = $bmp.Height

$dstDir = 'C:\Users\radit\LittleElsewhere\Assets\Resources\Menu_Components'
if (-not (Test-Path $dstDir)) { New-Item -ItemType Directory -Path $dstDir }

# Function to crop and save a region
function Save-Crop($x, $y, $cropW, $cropH, $filename) {
    if ($x + $cropW -gt $w) { $cropW = $w - $x }
    if ($y + $cropH -gt $h) { $cropH = $h - $y }
    if ($cropW -le 0 -or $cropH -le 0) { return }

    $rect = New-Object System.Drawing.Rectangle($x, $y, $cropW, $cropH)
    $cropped = $bmp.Clone($rect, $bmp.PixelFormat)
    $outPath = Join-Path $dstDir $filename
    $cropped.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $cropped.Dispose()
    Write-Host "Saved $filename ($cropW x $cropH) from ($x, $y)"
}

# Save Full High-Res Background / Layout Template
Save-Crop 0 0 $w $h 'Menu_Sheet_FullTemplate.png'

# Slice 4 main quadrants (Top-Left, Top-Right, Bottom-Left, Bottom-Right)
$halfW = [int]($w / 2)
$halfH = [int]($h / 2)

Save-Crop 0 0 $halfW $halfH 'Panel_Paper_Standard.png'
Save-Crop $halfW 0 $halfW $halfH 'Panel_Paper_Wide.png'
Save-Crop 0 $halfH $halfW $halfH 'Dialogue_Box_Template.png'
Save-Crop $halfW $halfH $halfW $halfH 'SaveSlot_Card.png'

# Slice Button Templates (from UI rows)
$thirdW = [int]($w / 3)
$quarterH = [int]($h / 4)

Save-Crop 0 0 $thirdW 160 'Button_Normal.png'
Save-Crop $thirdW 0 $thirdW 160 'Button_Hover.png'
Save-Crop ($thirdW * 2) 0 $thirdW 160 'Button_Pressed.png'
Save-Crop 0 160 $thirdW 160 'Button_Disabled.png'

$bmp.Dispose()
Write-Host "Menu components sliced and generated from menus.png!"
