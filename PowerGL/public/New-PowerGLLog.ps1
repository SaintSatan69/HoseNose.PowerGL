function New-PowerGLLog {
    param(
        [string]$message
    )
    $Script:ModuleLogger.Log("$($message)")
}