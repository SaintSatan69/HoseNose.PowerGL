<#
.SYNOPSIS
A function to make sending requests to the HTTP Server inside the poweGL engine

.DESCRIPTION
Uses Invoke-webrequest to talk to the /api/shapes/ of the engine and is merely a nice wrapper for the formating of the data for what your trying to do.

.PARAMETER ComputerName
The computer thats currently hosting the rendering engine, defaults to localhost.

.PARAMETER port
The port that the engine will listen on, defaults to 6969.

.PARAMETER Property
The property you'd like to retrive or modify on a shape, if left empty along with ShapeNumber it will gather everything for each shape .

.PARAMETER ShapeNumber
The shapesnumber, important to rember as this is its ID inside the engine.

.PARAMETER ValueX
The X part of the property, or the complete value if "Scale" is the property.

.PARAMETER valueY
The Y part of the property, not needed if "Scale" is the property.

.PARAMETER valueZ
The Z part of the property, not needed if "Scale" is the propert.

.PARAMETER Method
The Action being sent to the engine either "Get" to retrive data out of the engine, or "Post" to modify engine state.

.EXAMPLE
Invoke-PowerGLApiRequest -ShapeNumber 0 -Property Position -ValueX 1.0 -ValueY 0.0 -ValueZ 5.35. -Method Post

.EXAMPLE
Invoke-PowerGLApiRequest -Method Get

.EXAMPLE
Invoke-PowerGLApiRequest -Method Get -ShapeNumber 0 -Property Rotation

.NOTES
The Engine Config has to make sure the HTTP API is enabled otherwise this will not work.
#>
function Invoke-PowerGLApiRequest {
    [CmdletBinding()]
    param(
        [string]$ComputerName = "localhost",
        [int]$port = 6969,
        [ValidateSet("Position", "Rotation", "Scale", "Stretch", "Shear", "Momentum")]
        [string]$Property,
        [int]$ShapeNumber,
        [Single]$ValueX,
        [Single]$valueY = 0,
        [Single]$valueZ = 0,
        [ValidateSet("Get", "Post")]
        [parameter(Mandatory)]
        [string]$Method
    )
    Write-Debug "Debug Information `n{
    TargetMachine:$($ComputerName)
    TargetPort:$($port)
    HttpMethod:$($Method)
    ShapeNumber:$($ShapeNumber)
    ShapeProperty:$($Property)
}"

    if ($Method -eq "Post") {
        if ($Property -eq "Rotation") {
            Write-Debug "Changing name of the Rotation to the one that the engine looks for"
            $InnerProperty = "Rotate"
        }
        else {
            $InnerProperty = $Property
        }
        $Payload = @"
{
    "ShapeNumber":  $($ShapeNumber),
    "Property": `"$($InnerProperty)`",
    "ValueX": $($ValueX),
    "ValueY": $($valueY),
    "ValueZ": $($valueZ)
}
"@
        Write-Debug "Sending`n$($Payload)"
        $response = Invoke-WebRequest -Uri "http://$($ComputerName):$($port)/api/shapes/" -Method Post -Body $Payload
        return $response
    }
    elseif ($Method -eq "Get") {
        if ([string]::IsNullOrEmpty($Property) -or [string]::IsNullOrEmpty($ShapeNumber)) {
            Write-Debug "Property || shapenumber Sucessfully Validated to be null/or empty"
            $response = Invoke-WebRequest -Uri "http://$($ComputerName):$($port)/api/shapes/" -Method Get
            Write-Debug ([System.Text.Encoding]::UTF8.GetString($response.Content))
            return $response
        }
        elseif (![string]::IsNullOrEmpty($Property) -and ![string]::IsNullOrEmpty($ShapeNumber)) {
            Write-Debug "Property && shapenumber Sucessfully validated to not be null/or empty"
            $Payload = @"
            {
                "ShapeNumber": $($ShapeNumber),
                "Property":  $($Property)
            }
"@
            $response = Invoke-WebRequest -Uri "http://$($ComputerName):$($port)/api/shapes/" -Method Get -Body $Payload
            Write-Debug ([System.Text.Encoding]::UTF8.GetString($response.Content))
            return $response
        }
    }
}