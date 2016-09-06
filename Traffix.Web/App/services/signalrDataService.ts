module Traffix.Services {
    export class SignalrDataService {
        static $inject = ['$', '$rootScope'];

        constructor(private $: JQueryStatic, $rootScope: ng.IRootScopeService) {

            // creates a new hub connection
            var connection = $.hubConnection("/signalr", { useDefaultPath: false });

            // enabled logging to see in browser dev tools what SignalR is doing behind the scenes
            connection.logging = true;

            // create a proxy
            this.proxy = connection.createHubProxy('trafficHub');
            this.proxy.connection.logging = true;

            // start connection
            connection.start();

            // publish an event when server pushes a newCounters message for client
            this.proxy.on('updateMap', region => {
                $rootScope.$emit('updateMap', region);
            });
        }

        proxy: SignalR.Hub.Proxy;
    }
} 