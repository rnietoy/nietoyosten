
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

function getAlbumFolder() {
    return window.location.pathname.split('/')[3];
}

function createProgressBar() {
    return $('<div class="progress"><div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;">0%</div></div>');
}

function setProgressVar(pbDiv, value) {
    var innerDiv = pbDiv.find('.progress-bar');
    innerDiv.attr('aria-valuenow', value);
    innerDiv.attr('style', 'width: ' + value + '%;');
    innerDiv.text(value + '%');
}

function getEndFileUploadHandler(files, fileIndex) {
    return function (response) {
        // Set the upload status for this file (success, error, etc.)
        $('td:contains("' + files[fileIndex].name + '")').next().next().text(response);

        if (fileIndex < files.length - 1) {
            // Start uploading next file
            fileIndex++;
            var reader = new FileReader();
            reader.onload = getOnloadHandler(files, fileIndex);
            reader.readAsArrayBuffer(files[fileIndex]);
        } else {
            // We are done!
        }
    };
}

function getSendNextChunkFunction(arrayBuffer, files, fileIndex) {
    return function (response) {
        var rdto = response;

        // Check the errorMsg property. If it is not null we can proceed. Otherwise, an error occurred,
        // and we must report it in the page so the user can see it.
        if (rdto.errorMsg != null) {
            $('#ErrorMessage').text('Error: ' + rdto.errorMsg);
            $('#ErrorMessage').show();
            return;
        }

        // Update progress bar
        var percentDone = Math.floor((rdto.position) * 100 / arrayBuffer.byteLength);
        var pb = $('#progress' + fileIndex);
        setProgressVar(pb, percentDone);

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
                url: "/pictures/EndFileUpload",
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
            url: "/pictures/UploadFileChunk",
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
        var pb = createProgressBar();
        pb.attr("id", "progress" + fileIndex);
        $('td:contains("' + files[fileIndex].name + '")').next().append(pb);

        // Get first chunk of data
        var chunkSize = 50000;
        if (arrayBuffer.byteLength < chunkSize) {
            chunkSize = arrayBuffer.byteLength;
        }

        var chunk = new Uint8Array(arrayBuffer, 0, chunkSize);

        var dto = {
            folderName: getAlbumFolder(),
            fileName: files[fileIndex].name,
            base64Data: getBase64FromByteArray(chunk)
        };

        $.ajax({
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/pictures/BeginFileUpload",
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
        $('#FileTable tr[id^="row"]').remove();

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

        // Start uploading files
        var reader = new FileReader();
        reader.onload = getOnloadHandler(window.imageFiles, 0);
        reader.readAsArrayBuffer(window.imageFiles[0]);
    });
});