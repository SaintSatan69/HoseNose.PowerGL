<#
    .DESCRIPTION 
    A module that lets you do very rudimentary OpenGL without having to deal with a lot of the lower level parts. It is techically not all in powershell as Silk.Net.OpenGL doesn't work in powershell due to powershell
    not being able to support Silk.Net.Windowing. So powershell is meerly the Way to create your cubes (for now), and translate them in 3d Space. There is a Camera for movement in 3d space. Have Fun!!!

#>
# note for the crackhead in charge of dev make sure to change this to the C:\program version or maybe just dump this whole thing into its module folder
$PowerGL_DLL = "C:\Github\HoseRenderer\bin\Debug\net8.0\HoseRenderer.dll"
Import-Module $PowerGL_DLL

[HoseRenderer.MainRenderer]::PrintFunnyLogo()

foreach ($file in (Get-ChildItem $PSScriptRoot\public\*.ps1)) {
    . $file.FullName
}

#another note to me when i wake up and forgot everything i did, WE WON'T NEED TO MAKE A FUNCTION FOR WRITING TO NAMED PIPE, JUST USE THE .WRITEDIRECTIVE METHOD ON THE NAMEDPIPESERVER OBJECT RETURNED FROM INITALIZE-POWERGL
#PHASE 3 WITH THE SHAPE OBJECTS TO BE WRITTEN.