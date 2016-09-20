module Traffix.Services {

    export interface ISignalrDataService {
        connect();
        isConnecting(): boolean;
        isConnected(): boolean;
        connectionState();
    }

    export class SignalrDataService implements ISignalrDataService {
        static $inject = ['$rootScope'];

        connection = null;
        proxy = null;

        constructor(private $rootScope) {

        }

        connect() {
            
        }

        isConnecting(): boolean {
            return this.connection.state === 0;
        }

        isConnected(): boolean {
            return this.connection.state === 1;
        }

        connectionState() {
            return this.connection.state;
        }
    }
} 