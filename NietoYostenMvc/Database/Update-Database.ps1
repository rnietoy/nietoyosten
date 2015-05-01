# Update-Database script

param ([string]$ServerInstance,
        [string]$Database,
        [System.Management.Automation.PSCredential]$Credential,
        [string]$Version = "latest")

. .\DbVersioningLib.ps1

$changes = Get-SchemaLogChanges
        
# Get ordered list of change scripts
$scripts = Get-ChangeScripts

# Output current version and target version
$currentVersion = Get-CurrentVersion $changes
Write-Host "Current version: $currentVersion"
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
