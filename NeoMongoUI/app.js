var app = angular.module('IndexApp', ['ui.tree']);

app.config(['$qProvider', function ($qProvider) {
    $qProvider.errorOnUnhandledRejections(false);
}]);


app.run(["$rootScope", "$window", function ($rootScope, $window) {

    window.loading_screen = window.pleaseWait({
        logo: "../Content/Images/FlyingBird.png",
        backgroundColor: 'rgb(255,255,255)',
        loadingHtml: "<div class='sk-spinner sk-spinner-wave'><div class='sk-rect1'></div><div class='sk-rect2'></div><div class='sk-rect3'></div><div class='sk-rect4'></div><div class='sk-rect5'></div></div>"
    });

    $rootScope.$on('$routeChangeStart',
        function () {
            $window.scrollTo(0, 0); //scroll to top of page after each route change
        });

    $rootScope.$on('stopLoading',
        function () {
            window.loading_screen.finish(); //scroll to top of page after each route change
        });

    $rootScope.$on('$locationChangeStart',
        function () {
            $window.scrollTo(0, 0); //scroll to top of page after each route change
        });
}]);