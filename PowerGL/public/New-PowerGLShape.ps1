function New-PowerGLShape() {
    [CmdletBinding()]
    param(
        [string]$shapename = "Cube",
        [float]$PosX = 0,
        [float]$posY = 0,
        [float]$posZ = 0,
        [parameter(Mandatory = $true)]
        [uint32]$shapenum,
        [UInt32]$reflective = 0,
        [uint32]$Glowing = 0,
        [string]$ShaderPath = "$($script:ProgramDirectory)\Shaders\shader.vert",
        [string]$FragmentPath = "$($script:ProgramDirectory)\Shaders\shader.frag",
        [string]$TexturePath = "$($script:ProgramDirectory)\Randompictures\white.png",
        [float]$RotX = 0,
        [float]$RotY = 0,
        [float]$RotZ = 0,
        [float]$Size = 1,
        [float]$StrX = 1,
        [float]$strY = 1,
        [float]$strZ = 1,
        [float]$ShrX = 0,
        [float]$ShrY = 0,
        [float]$ShrZ = 0,
        [uint32]$Collision = 0,
        [float]$InitalMomentumX = 0,
        [float]$InitalMomentumY = 0,
        [float]$InitalMomentumZ = 0,
        [float]$BoingFactor = 1,
        [bool]$IsEffectedByGravity = $false,
        [uint32]$Controllable = 0,
        [uint32]$player = 1,
        [bool]$IsModel = $false,
        [string]$ModelFile = "$($script:ProgramDirectory)\Model\Sphere.model"


    )
    if ($ShrZ -ne [float]0) {
        Write-Warning "Shearing on the Z axis is very broken it won't shear the shape correctly and will instead morph the shape into a pyramid like shape"
    }
    if ($BoingFactor -gt [float]1) {
        Write-Warning "BoingFactors > then 1 will cause the shape to accellerate per bounce, Which normally would be fine if this didn't use AABB (collison boxes) if it gets fast enough it will just clip through things"
    }
    $pos_vec = [System.Numerics.Vector3]::new($PosX, $posY, $posZ)
    $rot_vec = [System.Numerics.Vector3]::new($RotX, $RotY, $RotZ)
    $str_vec = [System.Numerics.Vector3]::new($StrX, $strY, $strZ)
    $shr_vec = [System.Numerics.Vector3]::new($ShrX, $ShrY, $ShrZ)
    $mom_vec = [System.Numerics.Vector3]::new($InitalMomentumX, $InitalMomentumY, $InitalMomentumZ)

    return [HoseRenderer.PowerGL.Shape]::new($shapename, $pos_vec, $shapenum, $reflective, $Glowing, $ShaderPath, $FragmentPath, $TexturePath, $rot_vec, $Size, $str_vec, $shr_vec, $Collision, $mom_vec, $BoingFactor, [bool]$IsEffectedByGravity, $Controllable, $player, $IsModel, $ModelFile)
}