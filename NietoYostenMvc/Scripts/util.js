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