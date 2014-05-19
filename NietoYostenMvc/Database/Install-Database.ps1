# Install-Database script

param ([string]$ServerInstance,
        [string]$Database,
        [System.Management.Automation.PSCredential]$Credential,
        [string]$Version = 'latest')

. .\DbVersioningLib.ps1

# Install baseline schema
Write-Host 'Installing baseline schema'
Execute-SqlScript 'NietoYostenDb_baseline.sql'
Write-Host 'Done installing baseline schema'

$changes = Get-SchemaLogChanges
$scripts = Get-ChangeScripts

Write-Host "Target version: $Version"

$scriptsToApply = Get-AvailableScripts $scripts $changes $Version

# Apply scripts
Write-Host "`nScripts to apply: "

if ($scriptsToApply -eq $null) {
  Write-Host "None"
  Write-Warning "`nNo scripts were applied."
  Exit
}

$scriptsToApply | % { Write-Host $_ }

Write-Host
$scriptsToApply | % {
  Apply-SqlScript $_
}