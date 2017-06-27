app.controller('indexController', function ($scope, $rootScope, $window, $http, $q, $exceptionHandler) {
    $rootScope.$broadcast('stopLoading');
});