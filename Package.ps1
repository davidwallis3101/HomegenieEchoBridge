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

function Test-Net45
{
    if (Test-Path ‘HKLM:SOFTWAREMicrosoftNET Framework SetupNDPv4Full’)
    {
		if (Get-ItemProperty ‘HKLM:SOFTWAREMicrosoftNET Framework SetupNDPv4Full’ -Name Release -ErrorAction SilentlyContinue)
		{
			return $True
		}
		return $False
    }
}

function replace-file-content([String] $path, [String] $replace, [String] $replaceWith)
{
	(Get-Content $path) | Foreach-Object {$_ -replace $replace,$replaceWith} | Out-File $path
}

Write-Host "Target Dir $($TargetDir)"
Write-Host "Project Name $($ProjectName)"
Write-Host "Project Dir $($ProjectDir)"

if (Test-Net45 -eq $false) {Write-Error ".Net 4.5 is required to create the archive." -ErrorAction stop}
Add-Type -assembly "system.io.compression.filesystem" -ErrorAction stop

$source = "$($TargetDir)"
$destination = "$($ProjectDir)Output\$($ProjectName).zip"

# Determine Assembly version to version the interface
$file = "$($TargetDir)\MIG-EchoBridge.dll"
Write-Host $file
$ass = [System.Reflection.Assembly]::LoadFile($file)
$v = $ass.GetName().Version;
$version = [string]::Format("{0}.{1:00}.{2:00}.{3}",$v.Major, $v.Minor, $v.Build, $v.Revision)
write-host "DLL Version is: $version"

# TODO: generate files for the package and ammend versions
# Published 2016-10-07T20:45:00Z

# TODO Need to do in output folder to stop overwriting template placeholders
#replace-file-content "$source\" "{Version}" $version


If(Test-path $destination) {
	Write-Host "Removing existing interface archive"
	Remove-item $destination -Force
}


# Create interface archive
[io.compression.zipfile]::CreateFromDirectory($Source, $destination) 

Write-Host "`nInterface zip created at: $($destination)`n"

#Todo fix this
$testHgSystemIp = "192.168.0.161"

# Invoke-webrequest to upload interface zip
#Write-Host "Uploading interface"
#$resp = Invoke-RestMethod -Uri "http://$testHgSystemIp/api/HomeAutomation.HomeGenie/Config/Interface.Import/" -Method Post -InFile $destination -ContentType "multipart/form-data" -verbose
#If ($resp.StatusCode -ne "200") { Write-Error "Error Installing Interface" -ErrorAction Stop}


#Install Interface
#Write-host "Installing Interface"
#$resp = Invoke-webrequest -uri "http://$testHgSystemIp/api/HomeAutomation.HomeGenie/Config/Interface.Install" -verbose
#If ($resp.StatusCode -ne "200") { Write-Error "Error Installing Interface" -ErrorAction Stop}