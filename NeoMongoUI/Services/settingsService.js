app.service("settingsService", function () {

    var environmentLinks = {
        local: {
            clientLink: "http://localhost:50994/index.html",
            serviceLink: "http://localhost:62425"
        },
        miv: {
            clientLink: "http://socialboosterappmiv.azurewebsites.net",
            serviceLink: "http://socialboosterservucemiv.azurewebsites.net"
        }
    };

    var currentEnvironment = "local";

    this.getServerLink = function () {;
        return environmentLinks[currentEnvironment].serviceLink + "/api/";
    }

    this.getClientLink = function () {
        return environmentLinks[currentEnvironment].clientLink + "#/main";
    }
});