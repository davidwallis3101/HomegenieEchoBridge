[CmdLetBinding()]
param
(   
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$TargetDir,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$ProjectName,
    
    [Parameter(Mandatory = $true, Position = 2)]
    [string]$ProjectDir

)

$TargetDir = $TargetDir.Replace('"',"")
$ProjectDir = $ProjectDir.Trim()

Write-Host "Target Dir $($TargetDir)"
Write-Host "Project Name $($ProjectName)"
Write-Host "Project Dir $($ProjectDir)"

Write-Host "Pre-Build Script"