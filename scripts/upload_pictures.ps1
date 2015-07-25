
Import-AzurePublishSettingsFile "C:\proj\NietoYosten\azure.publishsettings"

$storageAccountName = "nietoyosten"

$storageAccountKey = Get-AzureStorageKey "nietoyosten" | % { $_.Primary }
$context = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

$containerName = "pictures"
$albumsToSkip = "randc", "bandw", "camino"

function Get-PicturesFromDb
{
    $query = "select A.FolderName, P.FileName FROM Pictures P INNER JOIN Albums A ON A.ID = P.AlbumID"
    Invoke-Sqlcmd -Query $query -ServerInstance "je7pcvhn7a.database.windows.net" -Database "nietoyosten2" -Username $dbUser -Password $dbPwd
}

function Get-BlobName($dbRow)
{
    "{0}\{1}" -f $dbRow.FolderName, $dbRow.FileName
}

function Blob-Exists([string]$blobName)
{
    [bool]$found = $false
    try {
        $blob = Get-AzureStorageBlob -Blob $blobName -Container $containerName -Context $context -ErrorAction Stop
        $found = $true
    }
    catch [Microsoft.WindowsAzure.Commands.Storage.Common.ResourceNotFoundException]
    {
        $found = $false
    }

    return $found
}

function Search-LocalFile($dbRow)
{
    $searchPaths = "C:\randcwedding\color", "C:\Users\rniet_000\SkyDrive\Pictures", "C:\fotki"
    
    if ($dbRow.FolderName -eq 'bandw')
    {
        $searchPaths = "C:\randcwedding\bandw", "C:\Users\rniet_000\SkyDrive\Pictures"
    }
    

    foreach ($path in $searchPaths)
    {
        $files = Get-ChildItem -File $dbRow.FileName -Path $path -Recurse

        if ($files.Count -ge 1)
        {
            $files.FullName    
        }
    }
}

function Should-Skip($pic)
{
    [bool]$skip = $false
    foreach ($album in $albumsToSkip)
    {
        if ($pic.FolderName -match $album)
        {
            $skip = $true
            break;
        }
    }

    return $skip
}

function Pick-Result($pic, $results)
{
    foreach ($r in $results)
    {
        if ($r -match $pic.FolderName)
        {
            return $r
        }
    }

    return $results[0]
}

function Upload-Pictures
{
    $pictures = Get-PicturesFromDb
    foreach ($pic in $pictures)
    {
        if (Should-Skip $pic)
        {
            Write-Output "Skipping $($pic.FileName) because it is in skipped album $($pic.FolderName)"
            continue;
        }

        $result = Search-LocalFile $pic
        if ($result.Count -eq 0)
        {
            Write-Output "Could not find file '$($pic.FileName)' in local drive. Skipping."
            continue;
        }

        [string]$localFile = $null
        if ($result.Count -eq 1)
        {
            $localFile = $result
        }

        if ($result.Count -gt 1)
        {
            $localFile = Pick-Result $pic $result
            Write-Output "Found $($result.Count) files with name '$($pic.FileName)'. Using file '$localFile'"
        }

        $blobName = Get-BlobName $pic
        if (-not (Blob-Exists $blobName))
        {
            Write-Output "Uploading '$localFile' to blob '$blobName'"
            Set-AzureStorageBlobContent -Blob $blobName -Container $containerName -File $localFile -Context $context -Force
            Write-Output "Uploaded '$localFile' to blob '$blobName'"
        }
        else {
            Write-Output "Blob '$blobName' already exists. Did not upload."
        }
    }
}