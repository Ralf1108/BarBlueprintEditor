window.browserResize = {
    getDimensions: function () {
        return {
            width: window.innerWidth,
            height: window.innerHeight
        };
    },

    registerResizeCallback: function (dotNetHelper) {
        window.addEventListener('resize', function () {
            dotNetHelper.invokeMethodAsync('OnBrowserResize');
        });
    }
};