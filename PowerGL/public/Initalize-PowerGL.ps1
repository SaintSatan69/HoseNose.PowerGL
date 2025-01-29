function Initialize-PowerGL(){
    [CmdletBinding()]
    param(
        [int]$phase = 1,
        [array]$shapesarray
    )
    switch ($phase) {
        1 { 
            [HoseRenderer.SharedFileIPC]::InitalizeFileIPC() 
        }
        2{
            [HoseRenderer.SharedFileIPC]::WriteFileIPC($shapesarray)
        }
        3 {
            return [HoseRenderer.NamedPipes.NamedPipeServer]::new("PowerGL",[System.IO.Pipes.PipeDirection]::InOut)
        }
        Default {
            throw "Intialization Phase Not Provided or Invalid, Phase 1 is building the inital files to dump the shape objects onto disk for the other program. Phase 3 is the contruction of a NamedPipe for the Powershell process to control the rendering process's shapes"
        }
    }
    
}