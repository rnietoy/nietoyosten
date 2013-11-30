function get_baseurl() {
    var pathArray = window.location.pathname.split('/');
    var host = pathArray[2];
    var baseUrl = window.location.protocol + "//" + window.location.host;
    return baseUrl;
}

window.fbAsyncInit = function () {
    FB.init({
        appId: $('#fbAppId').val(), // App ID
        channelUrl: get_baseurl() + '/channel.html', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });

    // Additional init code here
    FB.getLoginStatus(function (response) {
        if (response.status === 'connected') {
            console.log('connected');
        } else if (response.status === 'not_authorized') {
            // not_authorized
            console.log('not authorized');
            //login();
        } else {
            // not_logged_in
            console.log('not logged in');
            //login();
        }
    });

};

function fblogin_onlogin() {
    console.log('FB login button has been pressed');

    FB.getLoginStatus(function (response) {
        if (response.status === 'connected') {
            console.log('connected');

            // Send signed request using a POST request
            var baseUrl = get_baseurl();

            var form = $('<form>');
            form.attr('method', 'post');
            form.attr('path', baseUrl + '/Login.aspx');

            var hiddenField = $('<input type="hidden">');
            hiddenField.attr('name', 'signed_request');
            hiddenField.attr('value', response.authResponse.signedRequest);

            form.append(hiddenField);
            $('body').append(form);
            form.submit();

        } else if (response.status === 'not_authorized') {
            // not_authorized
            console.log('not authorized');
            //login();
        } else {
            // not_logged_in
            console.log('not logged in');
            //login();
        }
    });
}

// Load the SDK AsynchronouslyW
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "http://connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
} (document));

