
function getBase64FromByteArray(data) {
    var str = ""
    for (var i = 0; i < data.length; i++)
        str += String.fromCharCode(data[i]);

    return btoa(str).split(/(.{75})/).join("\n").replace(/\n+/g, "\n").trim();
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function getEndFileUploadHandler(files, fileIndex) {
    return function (response) {
        // Set the upload status for this file (success, error, etc.)
        $('td:contains("' + files[fileIndex].name + '")').next().next().text(response.d);

        if (fileIndex < files.length - 1) {
            // Start uploading next file
            fileIndex++;
            var reader = new FileReader();
            reader.onload = getOnloadHandler(files, fileIndex);
            reader.readAsArrayBuffer(files[fileIndex]);
        } else {
            // Tell user we are done. Add link to return to album.
            var url = "ViewAlbum.aspx?AlbumId=" + getParameterByName("AlbumId");
            var link = $('<a>').attr("href", url).text("Go back to album");
            var line = $('<p>').text("Upload complete!").append(link);
            $('#TopParagraph').append(line);
        }
    };
}

function getSendNextChunkFunction(arrayBuffer, files, fileIndex) {
    return function (response) {
        var rdto = response.d;

        // Check the errorMsg property. If it is not null we can proceed. Otherwise, an error occurred,
        // and we must report it in the page so the user can see it.
        if (rdto.errorMsg != null) {
            $('#ErrorMessage').text('Error: ' + rdto.errorMsg);
            $('#ErrorMessage').show();
            return;
        }

        // Update progress bar
        var percentDone = (rdto.position) * 100 / arrayBuffer.byteLength;
        $('#progress' + fileIndex).progressbar("option", "value", percentDone);

        if (rdto.position >= arrayBuffer.byteLength) {
            // We are done uploading this file, call EndFileUpload

            var dto = {
                folderName: rdto.folderName,
                fileName: rdto.fileName
            };

            $.ajax({
                type: "post",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "/UploadPicture.aspx/EndFileUpload",
                data: JSON.stringify(dto),
                success: getEndFileUploadHandler(files, fileIndex)
            });
            return;
        }

        // Determine size of next chunk
        var chunkSize = 50000;
        if ((arrayBuffer.byteLength - rdto.position) < chunkSize) {
            chunkSize = arrayBuffer.byteLength - rdto.position;
        }

        var chunk = new Uint8Array(arrayBuffer, rdto.position, chunkSize);

        var dto = {
            folderName: rdto.folderName,
            fileName: rdto.fileName,
            base64Data: getBase64FromByteArray(chunk)
        };

        $.ajax({
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/UploadPicture.aspx/UploadFileChunk",
            data: JSON.stringify(dto),
            success: getSendNextChunkFunction(arrayBuffer, files, fileIndex)
        });
    };
}

// This function handles the onload event for a FileReader.
// It calls BeginFileUpload to initiate the file upload process,
// which will be done in chunks.
function getOnloadHandler(files, fileIndex) {
    return function (e) {
        var arrayBuffer = e.target.result;

        // Create and initialize progress bar on table for this file
        var pb = $('<div>').attr("id", "progress" + fileIndex);
        $('td:contains("' + files[fileIndex].name + '")').next().append(pb);
        pb.progressbar({ value: 0 });

        // Get first chunk of data
        var chunkSize = 50000;
        if (arrayBuffer.byteLength < chunkSize) {
            chunkSize = arrayBuffer.byteLength;
        }

        var chunk = new Uint8Array(arrayBuffer, 0, chunkSize);

        var dto = {
            albumId: getParameterByName("AlbumId"),
            fileName: files[fileIndex].name,
            base64Data: getBase64FromByteArray(chunk)
        };

        $.ajax({
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/UploadPicture.aspx/BeginFileUpload",
            data: JSON.stringify(dto),
            success: getSendNextChunkFunction(arrayBuffer, files, fileIndex)
        });
    };
}

// jQuery event wireups for the UploadPicture page.

$(document).ready(function () {
    $('#files').change(function (evt) {
        // Fill table with files        
        var files = evt.target.files; // FileList object

        // Filter out non-image files
        var filesToAdd = new Array();

        for (var i = 0, f; f = files[i]; i++) {
            if (!f.type.match('image.*')) continue;
            filesToAdd.push(f);
        }

        // Clear the table
        $('#FileTable tr[id^="row"').remove();

        for (var i = 0, f; f = filesToAdd[i]; i++) {
            // Add file to html table
            var newRow = $('<tr>').attr("id", "row" + i);
            $('<td>').text(f.name).appendTo(newRow);
            $('<td>').appendTo(newRow);
            $('<td>').appendTo(newRow);
            $('#FileTable').append(newRow);
        }

        // Make table visible
        $('#FileTable').show();

        // Add files to global array
        window.imageFiles = filesToAdd;
    });

    $('#UploadButton').click(function (evt) {
        var reader = new FileReader();
        reader.onload = getOnloadHandler(window.imageFiles, 0);
        reader.readAsArrayBuffer(window.imageFiles[0]);
    });
});