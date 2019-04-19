window.addEventListener("load", function () {
    if (abp.setting.getBoolean('App.UserManagement.IsCookieConsentEnabled')) {
        window.cookieconsent.initialise({
            "palette": {
                "popup": {
                    "background": "#3937a3"
                },
                "button": {
                    "background": "#e62576"
                }
            },
            "showLink": false,
            "content": {
                "message": app.localize("CookieConsent_Message"),
                "dismiss": app.localize("CookieConsent_Dismiss")
            }
        });
    }
});