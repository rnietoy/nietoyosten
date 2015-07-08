// ShowAlbum.js

// Event wireups
$(document).ready(function() {
    $('#lnkDelete').click(function(evt) {
        var checkboxes = $('input[type="checkbox"]:checked');
        var ids = [];
        checkboxes.each(function (index, element) {
            ids.push(element.id);
        });

        var dto = {
            pictureIds: ids
        }

        if (confirm("Are you sure you want to delete the selected picture(s)?")) {
            $.ajax({
                type: "post",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "/pictures/DeletePictures",
                data: JSON.stringify(dto),
                success: function (data) { window.location.reload(true); }
            });
        }
    });
});