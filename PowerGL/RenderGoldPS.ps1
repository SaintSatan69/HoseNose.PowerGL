Import-Module $psscriptroot\PowerGL.psm1
$s = [GC]::GetTotalAllocatedBytes($true)
$shapecount = 0
$Shape_array = @()
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posY 0.0 -posZ 0.0 -TexturePath "$($PSScriptRoot)\Randompictures\Powershell7_blue.png" -ShrX 2.0 -Size 2.0 -strZ 6.0 -strY 2.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posY 1.0 -posZ -2.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -RotX 45 -strZ 2.5 -Size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 1.0 -posY -5.0 -posZ 0.0 -TexturePath "$($PSScriptRoot)\Randompictures\gold_ps.png" -Size 2.0 -Collision 1 -InitalMomentumY 0.01 -Restitution 0.9 -IsEffectedByGravity:$true
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posY -0.1 -posZ -2.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -RotX -45 -strZ 2.5 -Size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posY -1.0 -posZ 2.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -strZ 3.0 -Size 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX -1.0 -posY 5.0 -posZ 2.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -FragmentPath "$($PSScriptRoot)\Shaders\Clouds_1.frag" -Size 100
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 1.5 -posY 5.0 -posZ 0.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -Size 2.0 -InitalMomentumY -0.01 -Restitution 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posY -20.0 -posZ 0.0 -TexturePath "$($PSScriptRoot)\Randompictures\grass_1.png" -Collision 1 -StrX 50 -strZ 50
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posY 5.0 -posZ 3.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -InitalMomentumY -0.01 -Restitution 0.5
$shapecount++
$Shape_array += $shape
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 3.0 -posY 5.0 -posZ 3.0 -TexturePath "$($PSScriptRoot)\Randompictures\white.png" -Collision 1 -Controllable 1 -player 1
$shapecount++
$Shape_array += $shape
$e = [gc]::GetTotalAllocatedBytes($true)
Write-Output "the end total of shapes is $shapecount using $($e - $s) bytes"
Initialize-PowerGL -phase 1
Initialize-PowerGL -phase 2 -shapesarray $Shape_array
Start-PowerGL -ProgramFlag "DEV_BUGLAND"
$pipe = Initialize-PowerGL -phase 3
$pipe_string = "DEBUG:PIPETALKED"
$pipe_len = $pipe.WriteDirective($pipe_string)
Write-Output "STRLEN:$($pipe_string.Length)::PIPLEN:$($pipe_len)"
Start-Sleep -Seconds 10
$pipe.WriteDirective((Move-PowerGLShape -scale 1 -shapenum 0 -Property "SCALE"))
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 360; $i++ ) {
    try {
        $pipe.WriteDirective((Move-PowerGLShape -X 0 -Z 0 -Y $i -shapenum 9 -Property "ROTATE"))
        Start-Sleep -Milliseconds 100
    }
    catch {
        exit 0
    }
}