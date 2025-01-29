function Start-PowerGL(){
    [cmdletbinding()]
    param(
        [string]$ProgramFlag = $null
    )
    $PowerGL_EXE = "C:\Github\HoseRenderer\bin\Debug\net8.0\HoseRenderer.exe"
    if($null -ne $ProgramFlag){
        Start-Process $PowerGL_EXE  -ArgumentList $ProgramFlag
    }else{
        Start-Processq $PowerGL_EXE
    }
}