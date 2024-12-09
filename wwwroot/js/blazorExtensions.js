window.blazorExtensions = {
    setCookie: function (name, value, days) {
        console.log("setCookie called with name:", name, "value:", value, "days:", days);
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },
    deleteCookie: function (name) {
        console.log("deleteCookie called with name:", name);
        document.cookie = name + '=; Max-Age=-99999999;';
    },
    getCookie: function (name) {
        console.log("getCookie called with name:", name);
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    loadPageState: function (pageUri) {
        console.log("loadPageState called with pageUri:", pageUri);
        const stateJson = blazorExtensions.getCookie(pageUri + "-state");
        return stateJson ? JSON.parse(stateJson) : {};
    },
    savePageState: function (pageUri, state) {
        console.log("savePageState called with pageUri:", pageUri);
        const stateJson = JSON.stringify(state);
        blazorExtensions.setCookie(pageUri + "-state", stateJson, 7);
    },
    checkForBreadcrumbsElement: function () {
        const elementExists = document.getElementById('breadcrumbs') !== null;
        console.log("checkForBreadcrumbsElement called, element exists:", elementExists);
        return elementExists;
    },
    observeBreadcrumbsElement: function (dotNetHelper) {
        const observer = new MutationObserver((mutationsList, observer) => {
            for (let mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    const elementExists = document.getElementById('breadcrumbs') !== null;
                    if (elementExists) {
                        console.log("Breadcrumbs element detected in DOM.");
                        dotNetHelper.invokeMethodAsync('BreadcrumbsElementDetected');
                        observer.disconnect();
                        break;
                    }
                }
            }
        });

        observer.observe(document.body, { childList: true, subtree: true });
    }
};
