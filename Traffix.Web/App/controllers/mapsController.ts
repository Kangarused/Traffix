module Traffix.Controllers {
    export class MapsController {
        static $inject = ['$scope', '$window', '$rootScope', '$timeout', 'NgMap', 'trafficDataService', 'colorService'];
        googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCmq2yT3zYiQ1R6uWcP5mB_R_8WnZeJySQ&callback=initMap";
        defaultLat = -12.4463818;
        defaultLong = 130.9157756;
        defaultZoom = 3;
        headerHeight = 48;

        map;
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

            $scope.$watch(() => {return this.snappedMeterCounter}, () => {
                if (this.snappedMeterCounter != 0) {
                    if (this.snappedMeterCounter == this.allMeters.length) {
                        for (var i = 0; i < this.meters.length; i++) {
                            if (this.meters[i].linkedMeters.length > 1) {
                                for (var x = 1; x < this.meters[i].linkedMeters.length; x++) {
                                    this.displayRouteForMarkers(this.meters[i].linkedMeters[x - 1], this.meters[i].linkedMeters[x]);
                                }
                            }
                        }
                        this.snappedMeterCounter = 0;
                    }  
                }
            });
        }

        initMap(map) {
            this.map = map;
            this.loadTrafficData();

            this.map.addListener('zoom_changed', () => {
                this.adjustPopupPosition();
            });
        }

        updateMap(region) {
            //todo: update map if region matches current region
        }

        loadTrafficData() {
        this.trafficDataService.getLinkedTrafficMeters().then(
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
                    directionsService.route(request, this.updateMeters(i, x));
                }
            }
        }

        updateMeters = (group, index) => {
            return (response, status) => {
                if (status === google.maps.DirectionsStatus.OK) {
                    var pos = response.routes[0].legs[0].start_location;
                    this.meters[group].linkedMeters[index].latitude = pos.lat();
                    this.meters[group].linkedMeters[index].longitude = pos.lng();
                    this.snappedMeterCounter++;
                    this.$scope.$apply();
                }
            }
        }

        displayRouteForMarkers(start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
            var directionsService = new google.maps.DirectionsService();
            var startPoint = { lat: start.latitude, lng: start.longitude };
            var endPoint = { lat: end.latitude, lng: end.longitude };

            var request = {
                origin: startPoint,
                destination: endPoint,
                travelMode: google.maps.TravelMode.DRIVING
            }

            directionsService.route(request, this.drawRoutes(start, end));
        }

        drawRoutes(start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
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