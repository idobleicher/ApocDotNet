app.directive('indexTablesDirective', ['DALSrv', "$http", function (DALSrv, $rootScope, $window, $http) {
    return {
        restrict: 'EAC',
        replace: true,
        scope: {},
        templateUrl: "/Directives/HomePage/IndexNavBarDirective/indexNavBarDirective.html",
        controller: function ($scope, $http) {
            $scope.mongoTable = "";
            $scope.neoTable = "";
            DALSrv.getMongoCollectionsNames().then(function (data) {
                $scope.mongoTable = data;
            }), (function (error) {
                sweetAlert("אופס...", error.message, "error");
            });

            DALSrv.getNeoLabelsNames().then(function (data) {
                $scope.neoTable = data;
            }), (function (error) {
                sweetAlert("אופס...", error.message, "error");
            });
        }
    }
}]);