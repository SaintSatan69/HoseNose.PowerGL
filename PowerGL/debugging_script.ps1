import-module C:\Github\HoseRenderer\PowerGL\PowerGL.psm1
$bruh = @()
$shape = New-PowerGLShape -shapename "Cube" -shapenum 0 -PosX 1.0 -posy 2.0 -posz 3.0 -RotX 60.0 -RotY 45.0 -RotZ 30.0
$bruh += $shape
initialize-PowerGL -phase 1
initialize-PowerGL -phase 2 -shapesarray $bruh
Start-PowerGL
initialize-PowerGL -phase 3