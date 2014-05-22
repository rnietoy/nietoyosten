# Get-DatabaseStatus script

param ([string]$ServerInstance,
        [string]$Database,
        [System.Management.Automation.PSCredential]$Credential)

. .\DbVersioningLib.ps1

function Get-Version ($changeLogRecord) {
  "$($changeLogRecord.MajorReleaseNumber).$($changeLogRecord.MinorReleaseNumber).$($changeLogRecord.DotReleaseNumber)"
}

# Get schema history
$changes = Get-SchemaLogChanges

Write-Host "Schema history:"
$changes | % {
  $version = Get-Version $_
  Write-Host "$version $($_.ScriptName) on $($_.DateApplied)"
}
Write-Host

# Display current version
$currentVersion = Get-CurrentVersion $changes
Write-Host "Current version: $currentVersion`n"

Write-Host 'The following updates are available:'
$scripts = Get-ChangeScripts
$available = Get-AvailableScripts $scripts $changes 'latest'

if ($available -eq $null) {
  Write-Host "None"
} else {
  $available | % { Write-Host "  $_" }
}