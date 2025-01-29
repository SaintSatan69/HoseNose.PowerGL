import-module C:\Github\HoseRenderer\PowerGL\PowerGL.psm1
$s = [GC]::GetTotalAllocatedBytes($true)
$shapecount = 0
$Shape_array = @()
$tex_baspath = "C:\Github\HoseRenderer\Randompictures\Stress_test\"
$texture_array = @("andew.png","brain_damage.png","car.png","frodo.png","goose.png","helmet.png","hmm.png","max.png","max_hays.png","random_snake_1.png","random_snake_2.png","random_snake_3.png","spooder.png","THREAD.png")
for($j=0;$j -lt 8000;$j++){
    $texture_path = $tex_baspath + $texture_array[(Get-Random -Minimum 0 -Maximum ($texture_array.Length - 1))]
    [float]$ran_posx = Get-Random -Minimum -50 -Maximum 50
    [float]$ran_posy = Get-Random -Minimum -50 -Maximum 50
    [float]$ran_posZ = Get-Random -Minimum -50 -Maximum 50
    [float]$ran_rotx = Get-Random -Minimum 0 -Maximum 365
    [float]$ran_roty = Get-Random -Minimum 0 -Maximum 365
    [float]$ran_rotz = Get-Random -Minimum 0 -Maximum 365
    [float]$ran_scale = (Get-Random -Minimum 1 -Maximum 50) / 10
    [float]$ran_strX = (Get-Random -Minimum 1 -Maximum 50) /  10
    [float]$ran_strY = (Get-Random -Minimum 1 -Maximum 50) / 10
    [float]$ran_strZ = (get-random -Minimum 1 -Maximum 50) / 10
    $shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX $ran_posx -posy $ran_posy -posz $ran_posZ -texturepath $texture_path -RotX $ran_rotx -RotY $ran_roty -RotZ $ran_rotz -size $ran_scale -StrX $ran_strX -StrY $ran_strY -StrZ $ran_strZ
    $shapecount++
    $Shape_array += $shape
}
$e = [gc]::GetTotalAllocatedBytes($true)
Write-Output "the end total of shapes is $shapecount using $($e - $s) bytes"
initialize-PowerGL -phase 1
initialize-PowerGL -phase 2 -shapesarray $Shape_array
Start-PowerGL -programflag "pipe_enable"
$pipe = initialize-PowerGL -phase 3
$pipe_string = "DEBUG:PIPETALKED"
$pipe_len = $pipe.WriteDirective($pipe_string)
Write-Output "STRLEN:$($pipe_string.Length)::PIPLEN:$($pipe_len)"
start-sleep -Seconds 10
$pipe.WriteDirective((Move-PowerGLShape -scale 1 -shapenum 0 -property "SCALE"))
Start-Sleep -Milliseconds 100
$dirtective_array = @("TRANSFORM","SCALE","ROTATE")
$buff_len = 0
while($true){

    [string]$thing = $dirtective_array[(Get-Random -Minimum 0 -Maximum ($dirtective_array.Length - 1))]
    if($thing -eq "TRANSFORM" ){
        [float]$super_X = Get-Random -Minimum -100.0 -Maximum 100.0
        [float]$super_Y = Get-Random -Minimum -100.0 -Maximum 100.0
        [float]$super_Z = Get-Random -Minimum -100.0 -Maximum 100.0
        try{
            $buff_len = $pipe.WriteDirective((Move-PowerGLShape -X $super_X -Z $super_Z -Y $super_Y -shapenum (Get-Random -Minimum 0 -Maximum 7999) -property $thing))
            start-sleep -Milliseconds 100
        }
        catch{
            exit 0
        }
    }elseif($thing -eq "SCALE"){
        [float]$super_X = Get-Random -Minimum 0.1 -Maximum 20.0
        [float]$super_Y = Get-Random -Minimum 0.1 -Maximum 20.0
        [float]$super_Z = Get-Random -Minimum 0.1 -Maximum 20.0
        $scale = ($super_x + $super_Y - $super_Z ) / 2
        try{
            $buff_len = $pipe.WriteDirective((Move-PowerGLShape -scale $scale -shapenum (Get-Random -Minimum 0 -Maximum 7999) -property $thing))
            start-sleep -Milliseconds 100
        }
        catch{
            exit 0
        }
    }else{
        [float]$super_X = Get-Random -Minimum 0.1 -Maximum 360.0
        [float]$super_Y = Get-Random -Minimum 0.1 -Maximum 360.0
        [float]$super_Z = Get-Random -Minimum 0.1 -Maximum 360.0
        try{
            $buff_len = $pipe.WriteDirective((Move-PowerGLShape -X $super_X -Z $super_Z -Y $super_Y -shapenum (Get-Random -Minimum 0 -Maximum 7999) -property $thing))
            start-sleep -Milliseconds 100
        }
        catch{
            exit 0
        }
    }
    [System.Console]::Write("$buff_len ")
}
<#
for($i=0;$i -lt 360;$i++ ){
    try{
        $pipe.WriteDirective((Move-PowerGLShape -X 0 -Z 0 -Y $i -shapenum (Get-Random -Minimum 0 -Maximum 7999) -property "ROTATE"))
        start-sleep -Milliseconds 100
    }
    catch{
        exit 0
    }
}
#>