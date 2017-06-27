app.directive('indexNavBarDirective', ['DALSrv', "$http", function (DALSrv, $rootScope, $window, $http) {
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
            }),(function (error) {
                sweetAlert("אופס...", error.message, "error");
            });

            DALSrv.getNeoLabelsNames().then(function (data) {
                $scope.neoTable = data;
            }), (function (error) {
                sweetAlert("אופס...", error.message, "error");
            });

            $scope.mongoTableData = "";
            $scope.neoTableData = "";

            $scope.LoadMongoTable =
                function (key) {
                    DALSrv.getMongoCollectionsTableData(key).then(function (data) {
                        $scope.mongoTableData = data;
                    }), (function (error) {
                        sweetAlert("אופס...", error.message, "error");
                    });
                };

            $scope.LoadNeoTable =
                function (key) {
                    DALSrv.getNeoLabelsTableData(key).then(function (data) {
                        $scope.neoTableData = data;
                    }), (function (error) {
                        sweetAlert("אופס...", error.message, "error");
                    });
                };
        }
    }
}]);