function Initialize-PowerGL() {
    [CmdletBinding()]
    param(
        [int]$phase = 1,
        [array]$shapesarray
    )

    $pre_IPC_DIR = "$($Script:ProgramDirectory)\..\IPCFiles"
    $IPC_DIR = [System.IO.Path]::GetFullPath($pre_IPC_DIR)
    switch ($phase) {
        1 { 
            [HoseRenderer.SharedFileIPC]::InitalizeFileIPC($IPC_DIR) 
        }
        2 {
            [HoseRenderer.SharedFileIPC]::WriteFileIPC($IPC_DIR, $shapesarray)
        }
        3 {
            return [HoseRenderer.NamedPipes.NamedPipeServer]::new("PowerGL", [System.IO.Pipes.PipeDirection]::InOut)
        }
        Default {
            throw "Intialization Phase Not Provided or Invalid, Phase 1 is building the inital files to dump the shape objects onto disk for the other program. Phase 3 is the contruction of a NamedPipe for the Powershell process to control the rendering process's shapes"
        }
    }
    
}