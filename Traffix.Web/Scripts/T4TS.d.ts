﻿
/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module Traffix.Models {
    /** Generated from Traffix.Common.Dtos.TrafficLogsRequest **/
    export interface ITrafficLogsRequest {
        meterId: number;
        logs: Traffix.Models.ITrafficLog[];
    }
    /** Generated from Traffix.Common.Dtos.TrafficMetersList **/
    export interface ITrafficMetersList {
        meterIds: number[];
    }
    /** Generated from Traffix.Common.Model.TrafficLog **/
    export interface ITrafficLog {
        id: number;
        meterId: number;
        time: string;
        speed: number;
    }
    /** Generated from Traffix.Common.Model.TrafficMeter **/
    export interface ITrafficMeter {
        id: number;
        name: string;
        region: string;
        latitude: number;
        longitude: number;
        congestion: number;
        dateActive: string;
    }
    /** Generated from Traffix.Common.Model.TrafficLog **/
    export interface ITrafficLog {
    }
    /** Generated from Traffix.Common.Model.TrafficMeter **/
    export interface ITrafficMeter {
    }
}




