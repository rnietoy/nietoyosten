// Utility functions common to this site

function get_baseurl() {
    var pathArray = window.location.pathname.split('/');
    var host = pathArray[2];
    var baseUrl = window.location.protocol + "//" + window.location.host;
    return baseUrl;
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

function displayAlertMessage(message, alertClass) {
    var newAlert = true;
    var alertDiv = $('#alert-bar');
    if (alertDiv.length === 0) {
        alertDiv = $('<div>');
        alertDiv.attr('id', 'alert-bar');
        alertDiv.addClass('alert');
        alertDiv.addClass(alertClass);
        alertDiv.html('<a class="close" data-dismiss="alert" href="#" area-hidden="true">&times;</a>' + message);
    } else {
        newAlert = false;
        alertDiv.removeClass();
        alertDiv.addClass('alert');
        alertDiv.addClass(alertClass);
        alertDiv.html('<a class="close" data-dismiss="alert" href="#" area-hidden="true">&times;</a>' + message);
    }

    if (newAlert) {
        var alertRow = $('<div>');
        alertRow.addClass('row');
        alertRow.attr('id', 'msg-bar');
        alertRow.append(alertDiv);
        var bodyContent = $(".body-content");
        bodyContent.prepend(alertRow);
    }
}