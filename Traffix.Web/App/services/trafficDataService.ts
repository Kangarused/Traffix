module Traffix.Services {
    export interface ITrafficDataService {
        //Traffic Meters
        getTrafficMeters(): ng.IHttpPromise<Models.ITrafficMeter[]>;
        getLinkedTrafficMeters(): ng.IHttpPromise<Models.ILinkedTrafficMeters[]>;

        //Traffic Logs
        getTrafficLogsForMeter(meterId: number): ng.IHttpPromise<Models.ITrafficLog[]>;
        getTrafficLogsForMeters(meterIds: Models.ITrafficMetersList): ng.IHttpPromise<Models.ISortedTrafficLogs[]>;
    }

    export class TrafficDataService implements ITrafficDataService {

        static $inject = ['$http'];
        constructor(private $http: ng.IHttpService) {}

        //Traffic Meters
        getTrafficMeters(): ng.IHttpPromise<Models.ITrafficMeter[]> {
            return this.$http.get("/api/TrafficMeter/GetTrafficMeters");
        }

        getLinkedTrafficMeters(): ng.IHttpPromise<Models.ILinkedTrafficMeters[]> {
            return this.$http.get("/api/TrafficMeter/GetLinkedTrafficMeters");
        }

        //Traffic Logs
        getTrafficLogsForMeter(meterId: number): ng.IHttpPromise<Models.ITrafficLog[]> {
            return this.$http.get("/api/TrafficLog/GetTrafficLogsForMeter/" + meterId);
        } 

        getTrafficLogsForMeters(meterIds: Models.ITrafficMetersList): ng.IHttpPromise<Models.ISortedTrafficLogs[]> {
            return this.$http.post("/api/TrafficLog/GetTrafficLogsForMeters/", meterIds);
        } 
    }
}