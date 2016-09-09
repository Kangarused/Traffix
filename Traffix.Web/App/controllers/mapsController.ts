﻿module Traffix.Controllers {
    export class MapsController {
        static $inject = ['$scope', '$window', '$rootScope', '$timeout', 'NgMap', 'trafficDataService', 'colorService'];
        googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCmq2yT3zYiQ1R6uWcP5mB_R_8WnZeJySQ&libraries=places&callback=initMap";
        defaultLat = -12.4463818;
        defaultLong = 130.9157756;
        defaultZoom = 3;
        headerHeight = 48;

        map;
        searchBox;
        height: number;
        polyLines = [];

        allMeters: Models.ITrafficMeter[] = [];
        meters: Models.ILinkedTrafficMeters[];
        logs: Models.ISortedTrafficLogs[];
        snappedMeterCounter = 0;

        popupInfo = {
            name: null,
            congestion: null,
            header: null,
            avgSpeed: null,
            totalVehicles: null,
            timeBetween: null
        }

        colors = {
            low: [102, 204, 0],
            medium: [204, 153, 0],
            high: [204, 0, 0]
        }

        constructor(
            private $scope: IScope,
            private $window, 
            private $rootScope,
            private $timeout: ng.ITimeoutService,
            private ngMap,
            private trafficDataService: Services.ITrafficDataService,
            private colorService: Services.IColorService
        ) {
            $scope.vm = this;
            this.height = $window.innerHeight - this.headerHeight;

            angular.element($window).bind('resize', () => {
                this.height = $window.innerHeight - this.headerHeight;
                $scope.$apply();
            });

            $scope.$parent.$on('updateMap', (e, region) => {
                this.updateMap(region);
            });

            //$scope.$watch(() => {return this.snappedMeterCounter}, () => {
            //    if (this.snappedMeterCounter != 0) {
            //        if (this.snappedMeterCounter == this.allMeters.length) {
                        
            //            this.snappedMeterCounter = 0;
            //        }  
            //    }
            //});
        }

        initMap(map) {
            this.map = map;
            this.loadTrafficData("Northern Territory");

            //Prevent the map from being zoomed out too much and adjust the popup position
            this.map.addListener('zoom_changed', () => {
                if (this.map.getZoom() < 13) {
                    this.map.setZoom(13);
                }
                this.adjustPopupPosition();
            });

            //Get Searcbox input and use it to create a searchbox control
            var input = <HTMLInputElement>document.getElementById('traffixSearchBox');
            this.searchBox = new google.maps.places.SearchBox(input);

            //Updates the searchbox bounds which helps generate suggestions faster
            this.map.addListener('bounds_changed', () => {
                this.searchBox.setBounds(this.map.getBounds());
            });

            //Attach listener to the searchbox that will update the map position on search
            google.maps.event.addListener(this.searchBox, 'places_changed', () => {
                var places = this.searchBox.getPlaces();
                if (places.length == 0) {
                    return;
                }
                var bounds = new google.maps.LatLngBounds();
                places.forEach((place) => {

                    var region;
                    for (var i = 0; i < place.address_components.length; i++) {
                        if (place.address_components[i].types[0] == 'administrative_area_level_1') {
                            region = place.address_components[i].long_name;
                        }
                    }

                    this.updateMap(region);
                    if (place.geometry.viewport) {
                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                this.map.fitBounds(bounds);
            });
        }

        updateMap(region) {

            for (var i = 0; i < this.polyLines.length; i++) {
                this.polyLines[i].setMap(null);
            }
            this.polyLines = [];
            this.allMeters.length = 0;
            this.meters.length = 0;
            this.logs.length = 0;
            this.loadTrafficData(region);
        }

        loadTrafficData(region) {
            this.trafficDataService.getLinkedTrafficMeters(region).then(
            (response) => {
                this.meters = response.data;
                var list = {meterIds: []};
                for (var i = 0; i < this.meters.length; i++) {
                    for (var x = 0; x < this.meters[i].linkedMeters.length; x++) {
                        this.allMeters.push(this.meters[i].linkedMeters[x]);
                        list.meterIds.push(this.meters[i].linkedMeters[x].id);
                    }
                }
                this.trafficDataService.getTrafficLogsForMeters(list).then(
                (response) => {
                    this.logs = response.data;
                    this.snapMetersToRoads();
                });
            });
        }

        snapMetersToRoads() {
            var directionsService = new google.maps.DirectionsService();
            for (var i = 0; i < this.meters.length; i++) {
                for (var x = 0; x < this.meters[i].linkedMeters.length; x++) {

                    var latlng = { lat: this.meters[i].linkedMeters[x].latitude, lng: this.meters[i].linkedMeters[x].longitude };
                    var request = {
                        origin: latlng,
                        destination: latlng,
                        travelMode: google.maps.TravelMode.DRIVING
                    };

                    var lastMeter = (i === this.meters.length - 1);

                    directionsService.route(request, this.updateMeters(i, x, lastMeter));
                }
            }
        }

        updateMeters = (group, index, lastMeter: boolean) => {
            return (response, status) => {
                if (status === google.maps.DirectionsStatus.OK) {
                    var pos = response.routes[0].legs[0].start_location;
                    this.meters[group].linkedMeters[index].latitude = pos.lat();
                    this.meters[group].linkedMeters[index].longitude = pos.lng();
                    this.snappedMeterCounter++;
                    this.$scope.$apply();

                    if (lastMeter) {
                        //for (var i = 0; i < this.meters.length; i++) {
                        //    if (this.meters[i].linkedMeters.length > 1) {
                        //        for (var x = 1; x < this.meters[i].linkedMeters.length; x++) {
                        //            this.displayRouteForMarkers(this.meters[i].linkedMeters[x - 1], this.meters[i].linkedMeters[x]);
                        //        }
                        //    }
                        //}
                        this.displayRouteForMarkers(this.meters[i].linkedMeters[x - 1], this.meters[i].linkedMeters[x]);
                    }
                }
            }
        }

        displayRouteForMarkers(group, meter, start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
            var directionsService = new google.maps.DirectionsService();
            var startPoint = { lat: start.latitude, lng: start.longitude };
            var endPoint = { lat: end.latitude, lng: end.longitude };

            var request = {
                origin: startPoint,
                destination: endPoint,
                travelMode: google.maps.TravelMode.DRIVING
            }

            directionsService.route(request, this.drawRoutes(group, meter, start, end));
        }

        drawRoutes(group, meter, start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
            return (response, status) => {
                if (status === google.maps.DirectionsStatus.OK) {
                    var route = response;

                    var pathColor = this.colorService.RGB2Hex(this.getCongestionColor(start.congestion));
                    var currentColor = this.colorService.RGBtoHSV(this.getCongestionColor(start.congestion));
                    var endColor = this.colorService.RGBtoHSV(this.getCongestionColor(end.congestion));

                    var steps = route.routes[0].overview_path.length;
                    var increment = this.colorService.calculateIncrement(
                        currentColor, endColor, steps);

                    for (var i = 0; i < steps; i++) {
                        var path;
                        if (i + 1 >= steps) {
                            path = [route.routes[0].overview_path[i], route.routes[0].overview_path[i]];
                        } else {
                            path = [route.routes[0].overview_path[i], route.routes[0].overview_path[i+1]];
                        }
                            
                        var snappedPolyline = new google.maps.Polyline({
                            path: path,
                            strokeColor: pathColor,
                            strokeWeight: 7,
                            strokeOpacity: 1
                        });

                        snappedPolyline.setMap(this.map);
                        this.polyLines.push(snappedPolyline);
                        var transition = this.colorService.transition(currentColor, endColor, increment);
                        pathColor = transition.hex;
                        currentColor = transition.color;

                        meter++;

                        if (meter >= this.meters[group].linkedMeters.length) {
                            if (group < this.meters.length) {
                                group++;
                            } else {
                                group = null;
                            }
                            meter = 1;
                        }

                        if (group != null) {
                            this.displayRouteForMarkers(group, meter, this.meters[group].linkedMeters[meter - 1], this.meters[group].linkedMeters[meter]);
                        }
                    }
                }
            }
        }

        getCongestionColor(congestion) {
            if (congestion == 1) {
                return this.colors.medium;
            } else if (congestion == 2) {
                return this.colors.high;
            }
            return this.colors.low;
        }

        showPopup = (evt, meter: Models.ITrafficMeter) => {
            var id = 'meter-' + meter.id;

            this.popupInfo.name = meter.name;
            this.popupInfo.congestion = meter.congestion;
            this.popupInfo.header = this.getCongestionHeader(meter.congestion);
            this.popupInfo.avgSpeed = this.getAverageSpeed(meter);
            this.popupInfo.totalVehicles = this.getTotalVehicles(meter);

            this.map.showInfoWindow('popup', id);
        }

        getCongestionClass() {
            var congestion = this.popupInfo.congestion;
            if (congestion == 0) {
                return 'lowCongestion';
            } else if (congestion == 1) {
                return 'moderateCongestion';
            } else if (congestion == 2) {
                return 'highCongestion';
            }
            return '';
        }

        getCongestionHeader(congestion) {
            if (congestion == 0) {
                return 'Low Congestion';
            }else if (congestion == 1) {
                return 'Moderate Congestion';
            }else if (congestion == 2) {
                return 'High Congestion';
            }
            return 'Meter Statistics';
        }

        getAverageSpeed(meter: Models.ITrafficMeter) {
            var meterLogs = Enumerable.From(this.logs).First((x) => x.meterId == meter.id);
            var avgSpeed = 0;
            for (var i = 0; i < meterLogs.logs.length; i++) {
                avgSpeed += meterLogs.logs[i].speed;
            }
            avgSpeed = avgSpeed / meterLogs.logs.length;
            return avgSpeed;
        }

        getTotalVehicles(meter: Models.ITrafficMeter) {
            var meterLogs = Enumerable.From(this.logs).First((x) => x.meterId == meter.id);
            var totalVehicles = meterLogs.logs.length;
            return totalVehicles;
        }

        customisePopup() {
            if ($('.ng-map-info-window').length) {
                //Remove Padded Backgrounds
                var iwOuter = $('.gm-style-iw');
                var iwBackground = iwOuter.prev();
                iwBackground.children(':nth-child(2)').css({ 'display': 'none' });
                iwBackground.children(':nth-child(4)').css({ 'display': 'none' });

                //Reposition Window
                iwOuter.parent().parent().css({ left: '20px' });

                //Move the Arrow
                iwBackground.children(':nth-child(1)').attr('style', (i, s) => { return s + 'left: 145px !important;' });
                iwBackground.children(':nth-child(3)').attr('style', (i, s) => { return s + 'left: 145px !important;' });
                iwBackground.children(':nth-child(3)').find('div').children().css({ 'box-shadow': 'rgba(156, 156, 156, 0.6) 0px 1px 6px', 'z-index': '1' });

                var iwCloseBtn = iwOuter.next();
                iwCloseBtn.css({
                    opacity: '1',
                    right: '52px',
                    top: '17px'
                });
                iwCloseBtn.mouseout(function() {
                    $(this).css({ opacity: '1' });
                });

                this.adjustPopupPosition();
            }
        }

        adjustPopupPosition() {
            if ($('.ng-map-info-window').length) {
                var currentTop = $('.ng-map-info-window').position().top;
                var newTop = (currentTop - 40) + 'px';
                $('.ng-map-info-window').css('top', newTop);
            }
            this.$scope.$apply();
        }
    }
}