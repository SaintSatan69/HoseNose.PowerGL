function Move-PowerGLShape(){
    [CmdletBinding()]
    param(
        [uint]$shapenum = 0,
        [string]$Property = "Scale",
        [string]$X,
        [string]$Y,
        [string]$Z,
        [string]$scale
    )
    if($Property -ne "Scale"){
        $payload = $X + ":" + $Y + ":" + $Z 
    }else{
        $payload = $scale
    }
    $string = [string]$shapenum + "," + ($Property.ToUpper()) + "," + $payload
    return $string
}