 
var angularApplication = angular.module('traffix',
    [
        'ui.router',
        'ui.bootstrap',
        'datetimepicker',
        'ngSanitize',
        'validation',
        'validation.rule',
        'validation.schema',
        'PubSub',
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
    .controller('mapsController', Traffix.Controllers.MapsController);
