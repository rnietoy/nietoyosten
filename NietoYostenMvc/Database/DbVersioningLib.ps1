# Database version management common library

# Assumptions for database version management scripts:
#
# 1. The current version is determined as follows:
#    - Order the SchemaChangeLog table rows by MajorReleaseNumber, MinorReleaseNumber, DotReleaseNumber.
#    - The last row in this ordered result contains the current version.
#
# 2. All schema update scripts follow this pattern in their filename: "sc.MM.mm.ddd.sql"
#    MM - Major release number
#    mm - Minor release number
#    ddd - Dot release number
#
#    The only exception is the baseline / initial script.

$CurrentPath = $pwd

function Execute-Query ([string]$query, $variables) {
  if ($credential -eq $null) {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -ErrorAction stop -Query $query -Variable $variables
  } else {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -Username $cred.UserName -Password $cred.GetNetworkCredential().password -ErrorAction stop -Query $query -Variable $variables
  }
  cd $CurrentPath
}

function Execute-SqlScript ([string]$script) {
  if ($credential -eq $null) {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -InputFile $script
  } else {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -Username $cred.UserName -Password $cred.GetNetworkCredential().password -ErrorAction stop -InputFile $script
  }
  cd $CurrentPath
}

function Get-SchemaLogChanges {
  Execute-Query "select * from SchemaChangeLog order by MajorReleaseNumber, MinorReleaseNumber, DotReleaseNumber"
}

function Get-ChangeScripts {
  @("NietoYostenDb_baseline.sql") + (gci .\changes).Name | ? { $_ -match "sc\.\d\d\.\d\d\.\d\d\d\.sql" } | sort
}

function Get-CurrentScript($changes) {
  ($changes | Select-Object -Last 1).ScriptName
}

function Get-Version ($changeLogRecord) {
  "$($changeLogRecord.MajorReleaseNumber).$($changeLogRecord.MinorReleaseNumber).$($changeLogRecord.DotReleaseNumber)"
}

function Get-CurrentVersion($changes) {
  Get-Version ($changes | Select-Object -Last 1)
}

function Get-AvailableScripts($scripts, $changes, [string]$targetVersion) {
  $currentVerScript = ($changes | Select-Object -Last 1).ScriptName
  $curVerPos = $scripts.IndexOf($currentVerScript)
  
  # Determine position of target version
  if ($targetVersion -eq "latest") {
    $targetVerPos = $scripts.Length - 1
  } else {
    $targetVerPos = $scripts.IndexOf("sc.{0}.sql" -f $targetVersion)
    
    if ($targetVerPos -eq -1) {
      throw "Could not find script corresponding to target version"
    }
  }

  # Determine scripts that need to be applied
  if ($targetVerPos -lt $curVerPos) {
    throw "Target version is older than current version"
  }
  
  # Return null if target version is same as current version, since there are no scripts
  # to apply in this case.
  if ($targetVerPos -eq $curVerPos) {
    return $null
  } else {
    return $scripts[($curVerPos+1)..$targetVerPos]
  }
}

# Apply SQL script.
# $scriptFile - Name of schema change script to apply (without the path, i.e. 'sc.01.00.002.sql').
#               This function assumes the script is located in the 'changes' directory relative to the current dir.
function Apply-SqlScript([string]$scriptFile) {
  Write-Host "Applying script: $scriptFile"
  Execute-SqlScript  ".\changes\$scriptFile"
  
  # Update SchemaChangeLogTable
  $updateLogQuery = @"
    INSERT INTO [SchemaChangeLog]
       ([MajorReleaseNumber],[MinorReleaseNumber],[DotReleaseNumber],[ScriptName],[DateApplied])
    VALUES (`$(Major),`$(Minor),`$(Dot),`$(ScriptName),GETDATE())
"@
  
  # Extract major, minor, and dot build numbers from script filename
  $scriptFile -match 'sc\.(?<major>\d\d)\.(?<minor>\d\d)\.(?<dot>\d\d\d)\.sql' > $null
  $variables = "Major = '$($matches['major'])'",
               "Minor = '$($matches['minor'])'",
               "Dot = '$($matches['dot'])'",
               "ScriptName = '$scriptFile'"
  
  Execute-Query -Query $updateLogQuery -Variable $variables
  Write-Host "Done applying script: $scriptFile`n"
}