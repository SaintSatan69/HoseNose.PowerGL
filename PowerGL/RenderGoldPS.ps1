Import-Module $psscriptroot\PowerGL.psm1
$s = [GC]::GetTotalAllocatedBytes($true)
$shapecount = 0
$Shape_array = @()
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posy 0.0 -posz 0.0 -texturepath "$($PSScriptRoot)\Randompictures\Powershell7_blue.png" -ShrX 2.0 -size 2.0 -StrZ 6.0 -StrY 2.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posy 1.0 -posz -2.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -RotX 45 -Strz 2.5 -size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 1.0 -posy -5.0 -posz 0.0 -texturepath "$($PSScriptRoot)\Randompictures\gold_ps.png" -size 2.0 -Collision 1 -InitalMomentumY 0.01 -boingfactor 0.9 -IsEffectedByGravity:$true
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posy -0.1 -posz -2.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -RotX -45 -Strz 2.5 -size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posy -1.0 -posz 2.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -Strz 3.0 -size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posy 5.0 -posz 2.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -FragmentPath "$($PSScriptRoot)\Shaders\Clouds_1.frag" -size 100
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 1.5 -posy 5.0 -posz 0.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -size 2.0 -InitalMomentumY -0.01 -boingfactor 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posy -20.0 -posz 0.0 -texturepath "$($PSScriptRoot)\Randompictures\grass_1.png" -Collision 1 -StrX 50 -StrZ 50
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posy 5.0 -posz 3.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -InitalMomentumY -0.01 -boingfactor 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 3.0 -posy 5.0 -posz 3.0 -texturepath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -Controllable 1 -player 1
$shapecount++
$Shape_array += $shape
$e = [gc]::GetTotalAllocatedBytes($true)
Write-Output "the end total of shapes is $shapecount using $($e - $s) bytes"
initialize-PowerGL -phase 1
initialize-PowerGL -phase 2 -shapesarray $Shape_array
Start-PowerGL -programflag "pipe_enable"
$pipe = initialize-PowerGL -phase 3
$pipe_string = "DEBUG:PIPETALKED"
$pipe_len = $pipe.WriteDirective($pipe_string)
Write-Output "STRLEN:$($pipe_string.Length)::PIPLEN:$($pipe_len)"
Start-Sleep -Seconds 10
$pipe.WriteDirective((Move-PowerGLShape -scale 1 -shapenum 0 -property "SCALE"))
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 360; $i++ ) {
    try {
        $pipe.WriteDirective((Move-PowerGLShape -X 0 -Z 0 -Y $i -shapenum 9 -property "ROTATE"))
        Start-Sleep -Milliseconds 100
    }
    catch {
        exit 0
    }
}