<#
    .DESCRIPTION 
    A module that lets you do very rudimentary OpenGL without having to deal with a lot of the lower level parts. It is techically not all in powershell as Silk.Net.OpenGL doesn't work in powershell due to powershell
    not being able to support Silk.Net.Windowing. So powershell is meerly the Way to create your cubes (for now), and translate them in 3d Space. There is a Camera for movement in 3d space. Have Fun!!!

#>
# note for the crackhead in charge of dev make sure to change this to the C:\program version or maybe just dump this whole thing into its module folder
$PowerGL_DLL = $PSScriptRoot + "\HoseRenderer.dll"
Import-Module $PowerGL_DLL
#Add-Type $PowerGL_DLL
#To Create all folders in the user profile that they HAVE to have write permission to since the program generates Log files for debugging and the JSON files that contains the definition of the Shape objects on the filesystem so that you can fully see the definition while debugging the engine/JSON serializer
$user_writable_folders = @( "IPCFILES", "Logs" )
$USR_PROFILE = "$($env:USERPROFILE)\appdata\local\temp\PowerGL"
if (!([System.IO.Directory]::Exists($USR_PROFILE))) {
    New-Item $USR_PROFILE -ItemType Directory
}
for ([int]$i = 0; $i -lt $user_writable_folders.Length; $i++) {
    if (!([System.IO.Directory]::Exists("$($USR_PROFILE)\$($user_writable_folders[$i])"))) {
        New-Item "$($USR_PROFILE)\$($user_writable_folders[$i])" -ItemType Directory
    }
}
#FROM THIS POINT FORWARD YOU CAN CALL THE [HOSERENDERER.LOGGER]::NEW("",$null) AND USE FILE BASED LOG FILES THAT GO TO THE CURRENT USERS LOCAL\TEMP FOLDER FOR POWERGL

[HoseRenderer.MainRenderer]::PrintFunnyLogo()

$Script:ProgramDirectory = $PSScriptRoot

[string]$script:IPC_FOLDER = ($USR_PROFILE + "\IPCFiles")

[HoseRenderer.Logger]$Script:ModuleLogger = [HoseRenderer.Logger]::new("PowerShell", "C:\Users\$($env:USERNAME)\appdata\local\temp\powergl\logs")
foreach ($file in (Get-ChildItem $PSScriptRoot\public\*.ps1)) {
    . $file.FullName
}
New-PowerGLLog -message "PowerShell Module Imported"
$OnRemoveScript = {
    $hose = Get-Process | Where-Object { $_.Name -eq "HoseRenderer.exe" }
    while ($null -ne (Get-Process -Id $hose.Id)) {
        Write-Output "Cannot Cleanup the engine until it is closed"
        Start-Sleep -Milliseconds 10
    }
    Clear-PowerGL
}
$ExecutionContext.SessionState.Module.OnRemove += $OnRemoveScript
Register-EngineEvent -SourceIdentifier ([System.Management.Automation.PsEngineEvent]::Exiting) -Action $OnRemoveScript