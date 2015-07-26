// Login page javascript code


window.fbAsyncInit = function() {
    FB.init({
        appId: '357558851018126',
        channelUrl: get_baseurl() + '/Content/channel.html', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true // parse XFBML
    });
};

function fblogin_success() {
    return function (response) {
        window.location = response.RedirectUrl;
    }
}

function getReturnUrl() {
    var returnUrl = $('#return-url').val();
    if (returnUrl == "") {
        returnUrl = "/";
    }
    return returnUrl;
}

function fblogin_onlogin() {
    FB.getLoginStatus(function (response) {
        if (response.status === 'connected') {
            console.log('connected');

            var dto = {
                signed_request: response.authResponse.signedRequest,
                return_url: getReturnUrl()
            };

            $.ajax({
                type: "post",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "/Account/FbLogin",
                data: JSON.stringify(dto),
                success: fblogin_success()
            });

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

// Load the SDK Asynchronously
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "http://connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));