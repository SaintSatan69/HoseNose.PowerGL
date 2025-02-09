function Clear-PowerGL {
    Write-Output "Clean up is running"
    [hoserenderer.SharedFileIPC]::UninitalizeFileIPC($script:IPC_FOLDER)
}
