# Script to bulk upload pictures from fotki.

[CmdletBinding()]
param([System.Management.Automation.PSCredential]$Credential)

if ($PSBoundParameters['Debug']) {
    $DebugPreference = 'Continue'
}

# Initialization / globals
[reflection.assembly]::LoadWithPartialName("System.Drawing")

function Init-Azure
{
    Import-AzurePublishSettingsFile "C:\proj\NietoYosten\azure.publishsettings"
    $StorageAccountName = "nietoyosten"
    $StorageAccountKey = Get-AzureStorageKey "nietoyosten" | % { $_.Primary }
    $StorageContext = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey
    $ContainerName = "pictures"
}

$ServerInstance = "je7pcvhn7a.database.windows.net"
$Database = "nietoyosten2"
$DbUser = $Credential.UserName
$DbPwd = $Credential.GetNetworkCredential().password
$UserId = 10

function Get-DateTaken([string]$FileName)
{
    $pic = New-Object System.Drawing.Bitmap($FileName)
    $bytearr = $pic.GetPropertyItem(36867).Value
    $str = [System.Text.Encoding]::ASCII.GetString($bytearr)
    [datetime]::ParseExact($str,"yyyy:MM:dd HH:mm:ss`0",$Null)
    $pic.Dispose()
}

function Get-FolderDate([string]$FolderName)
{
    $files = Get-ChildItem -Path $FolderName -Filter "*.jpg"
    $dates = $files.FullName | % { Get-DateTaken $_ } | sort
    return $dates[0]
}

function Format-AlbumTitle([string]$FolderName)
{
    $parts = $FolderName -split "\\"
    $len = $parts.Count
    $parts[2..$len] -join " "
}

function Get-AllAlbums($RootFolder)
{
    $dirs = Get-ChildItem $RootFolder -Recurse -Directory
    foreach ($dirName in $dirs.FullName)
    {
        $files = Get-ChildItem $dirName -Filter "*.jpg"
        if ($files.Length -gt 0)
        {
            $dirName
        }
    }
}

function Blob-Exists([string]$BlobName)
{
    [bool]$found = $false
    try {
        $blob = Get-AzureStorageBlob -Blob $BlobName -Container $ContainerName -Context $StorageContext -ErrorAction Stop
        $found = $true
    }
    catch [Microsoft.WindowsAzure.Commands.Storage.Common.ResourceNotFoundException]
    {
        $found = $false
    }

    return $found
}

function UploadTo-Azure([string]$LocalFile, [string]$BlobName)
{
    if (-not (Blob-Exists $blobName))
    {
        Write-Output "Uploading '$LocalFile' to blob '$BlobName'"
        Set-AzureStorageBlobContent -Blob $blobName -Container $ContainerName -File $LocalFile -Context $StorageContext -Force
        Write-Output "Uploaded '$LocalFile' to blob '$BlobName'"
    }
    else {
        Write-Output "Blob '$BlobName' already exists. Did not upload."
    }
}

function Execute-SqlQuery([string]$Query)
{
    Write-Debug "Executing query: $Query"
    Invoke-Sqlcmd -Query $Query -ServerInstance $ServerInstance -Database $Database -Username $DbUser -Password $DbPwd
}

function Add-AlbumToDb([string]$LocalFolder)
{
    $dateTaken = Get-FolderDate $LocalFolder
    $title = Format-AlbumTitle $LocalFolder
    $query = "INSERT INTO Albums (Title,FolderName,CreatedBy,ModifiedBy,CreatedAt,ModifiedAt) VALUES ('$title','$title',$UserId,$UserId,'$dateTaken','$dateTaken')"
    Execute-SqlQuery $query
}

function Get-AlbumId([string]$LocalFolder)
{
    $title = Format-AlbumTitle $LocalFolder
    $query = "SELECT ID FROM Albums WHERE Title='$title'"
    $r = Execute-SqlQuery $query
    $r.ID
}

function Add-PictureToDb([int]$AlbumId, [System.IO.FileInfo]$LocalFile)
{
    $dateTaken = Get-DateTaken $LocalFile.FullName
    $fileName = $LocalFile.Name
    $query = "INSERT INTO Pictures (AlbumID,Title,FileName,UploadedAt,UploadedBy) VALUES ($AlbumID,'$fileName','$fileName','$dateTaken',$UserId)"
    Execute-SqlQuery $query
}

function Upload-Album([string]$LocalFolder)
{
    Add-AlbumToDb $LocalFolder
    $albumId = Get-AlbumId $LocalFolder
    $files = Get-ChildItem -Path $LocalFolder -Filter "*.jpg"
    $azureFolder = Format-AlbumTitle $LocalFolder

    foreach ($localFile in $files)
    {
        $blobName = "{0}\{1}" -f $azureFolder, $localFile.Name
        UploadTo-Azure $localFile.FullName $blobName
        Add-PictureToDb $albumId $localFile
    }
}

function Test
{
    UploadTo-Azure "D:\fotki\trips\mxico\aguaviva\IMG00062.jpg" "trips mxico aguaviva\IMG00062  with-spaces.jpg"
}

Init-Azure

#Add-AlbumToDb "D:\fotki\ani-alemania"

Upload-Album "D:\fotki\hyh\bastrop state park"
