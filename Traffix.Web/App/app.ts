 
var angularApplication = angular.module('traffix',
    [
        'ui.router',
        'ui.bootstrap',
        'ngSanitize',
        'show.when.loading',
        'ngCookies',
        'CaseFilter',
        'ngMap',
        'ngDomEvents'
    ])
    .config([
        '$stateProvider', '$urlRouterProvider', ($stateProvider, $urlRouterProvider) => {

            $urlRouterProvider.otherwise("/map");

            $stateProvider.state('map', {
                url: '/map',
                templateUrl: "App/views/maps.html",
                controller: "mapsController"
            });
        }
    ])
    .run([
        '$rootScope', ($rootScope) => {
            $rootScope.enumDescriptions = Traffix.Models.EnumLabelDictionary;
        }
    ])
    .service('trafficDataService', Traffix.Services.TrafficDataService)
    .service('signalrDataService', Traffix.Services.SignalrDataService)
    .service('colorService', Traffix.Services.ColorService)
    .controller('mapsController', Traffix.Controllers.MapsController);
