module Traffix.Services {
    export interface ITrafficDataService {
        //Traffic Meters
        getTrafficMeters(): ng.IHttpPromise<Models.ITrafficMeter[]>;

        //Traffic Logs
        getTrafficLogsForMeter(meterId: number): ng.IHttpPromise<Models.ITrafficLog[]>;
        getTrafficLogsForMeters(meterIds: Models.ITrafficMetersList): ng.IHttpPromise<Models.ITrafficLogsRequest[]>;
    }

    export class TrafficDataService implements ITrafficDataService {

        static $inject = ['$http'];
        constructor(private $http: ng.IHttpService) {}

        //Traffic Meters
        getTrafficMeters(): ng.IHttpPromise<Traffix.Models.ITrafficMeter[]> {
            return this.$http.get("/api/TrafficMeter/GetTrafficMeters");
        }

        //Traffic Logs
        getTrafficLogsForMeter(meterId: number): ng.IHttpPromise<Models.ITrafficLog[]> {
            return this.$http.get("/api/TrafficLog/GetTrafficLogsForMeter/" + meterId);
        } 

        getTrafficLogsForMeters(meterIds: Models.ITrafficMetersList): ng.IHttpPromise<Models.ITrafficLogsRequest[]> {
            return this.$http.post("/api/TrafficLog/GetTrafficLogsForMeters/", meterIds);
        } 
    }
}