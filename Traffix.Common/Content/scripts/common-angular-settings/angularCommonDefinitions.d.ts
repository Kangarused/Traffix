/// <reference path="../angular/angular.d.ts" />
/// <reference path="../angular/angular-cookies.d.ts" />
/// <reference path="../linqjs/linq.d.ts" />
/// <reference path="../angular-ui-router/angular-ui-router.d.ts" />
/// <reference path="../signalr/signalr.d.ts"/>
/// <reference path="../google-maps/google.maps.d.ts"/>

//global vars and functions
 
declare var escape: any;
declare var angularApplication: ng.IModule;
declare var emailAddresses: any;

declare module Traffix.Models {
    export interface IPagedResponse<T> {
        items: T[];
        numRecords: number;
    }
    export interface IResponseObject<T> {
        data: T;
    }

    export interface IActionResponse {
        succeed: boolean;
        errors: string[];
    }

    export interface IActionResponseGeneric<T> extends IActionResponse {
        response: T;
    }
}

 

declare module Traffix {

    export interface IEnumDescripitons {
        get(enumType:string, key:string):string;
    }

    export interface IScope extends ng.IScope {
        vm: any;
        enumDescriptions: IEnumDescripitons;
    }
}