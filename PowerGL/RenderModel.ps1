import-module C:\Github\HoseRenderer\PowerGL\PowerGL.psm1
$s = [GC]::GetTotalAllocatedBytes($true)
$shapecount = 0
$Shape_array = @()
$shape = New-PowerGLShape -shapename "Cube" -shapenum $shapecount -PosX 0.0 -posy 0.0 -posz 0.0 -texturepath "C:\Github\HoseRenderer\Randompictures\Stress_test\Max_hays.png" -ismodel:$true -ModelFile "C:\Github\HoseRenderer\Model\Sphere.model" -shaderpath "C:\Github\HoseRenderer\Shaders\Model.vert" -FragmentPath "C:\github\HoseRenderer\Shaders\Model.frag"
$shapecount++
$Shape_array += $shape
$e = [gc]::GetTotalAllocatedBytes($true)
Write-Output "the end total of shapes is $shapecount using $($e - $s) bytes"
initialize-PowerGL -phase 1
initialize-PowerGL -phase 2 -shapesarray $Shape_array
Start-PowerGL -programflag "DEV_BUGLAND"
$pipe = initialize-PowerGL -phase 3
$pipe_string = "DEBUG:PIPETALKED"
$pipe_len = $pipe.WriteDirective($pipe_string)
Write-Output "STRLEN:$($pipe_string.Length)::PIPLEN:$($pipe_len)"
[float]$pose_x = 0
while($true){
    try{
        [System.Console]::WriteLine("Sending Data $($pose_x) down the pipe line")
        $pipe.WriteDirective((Move-PowerGLShape -shapenum 0 -X $pose_x -Y 0.0 -Z 0.0 -Property "TRANSFORM"))
        start-sleep -Milliseconds 100
        $pose_x += 0.001
    }
    catch{
        Write-Warning "Writing Directive Died for reason $($Error[0])"
        exit 0
    }
}