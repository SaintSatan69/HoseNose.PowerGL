function Initialize-PowerGL() {
    [CmdletBinding()]
    param(
        [int]$phase = 1,
        [array]$shapesarray
    )
    $IPC_DIR = [System.IO.Path]::GetFullPath($script:IPC_FOLDER)
    switch ($phase) {
        1 { 
            [HoseRenderer.SharedFileIPC]::InitalizeFileIPC($IPC_DIR)
            New-PowerGLLog -message "IPC Phase 1 Completed"
        }
        2 {
            [HoseRenderer.SharedFileIPC]::WriteFileIPC($IPC_DIR, $shapesarray)
            New-PowerGLLog -message "IPC Phase 2 Written to Disk Successfully"
        }
        3 {
            return [HoseRenderer.NamedPipes.NamedPipeServer]::new("PowerGL", [System.IO.Pipes.PipeDirection]::InOut)
            New-PowerGLLog -message "IPC Named Pipe server Started"
        }
        Default {
            throw "Intialization Phase Not Provided or Invalid, Phase 1 is building the inital files to dump the shape objects onto disk for the other program. Phase 3 is the contruction of a NamedPipe for the Powershell process to control the rendering process's shapes"
        }
    }
}