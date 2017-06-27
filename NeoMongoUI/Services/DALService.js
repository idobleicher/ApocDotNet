app.service('DALSrv', ["$http", "settingsService", function ($http, settingsService) {

    this.getMongoCollectionsNames = function () {
        return $http.get(settingsService.getServerLink() + "Entities/Mongo");
    }

    this.getNeoLabelsNames = function () {
        return $http.get(settingsService.getServerLink() + "Entities/Neo");
    }

    this.getMongoCollectionsTableData = function (tableName) {
        return $http.get(settingsService.getServerLink() + "Entities/Mongo/" +tableName);
    }

    this.getNeoLabelsTableData = function (tableName) {
        return $http.get(settingsService.getServerLink() + "Entities/Neo/"+ tableName);
    }
}]);