module Traffix.Controllers {
    export class MapsController {
        static $inject = ['$scope', '$window', '$rootScope', '$timeout', 'NgMap', 'trafficDataService'];
        googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCmq2yT3zYiQ1R6uWcP5mB_R_8WnZeJySQ&callback=initMap";
        defaultLat = -12.4463818;
        defaultLong = 130.9157756;
        defaultZoom = 3;
        headerHeight = 48;

        map;
        height: number;
        gMarkers = [];
        polyLines = [];

        meters: Models.ITrafficMeter[];
        logs: Models.ITrafficLogsRequest[];

        popupInfo = {
            congestion: null,
            header: null,
            avgSpeed: null,
            totalVehicles: null,
            timeBetween: null
        }

        constructor(
            private $scope: IScope,
            private $window, 
            private $rootScope,
            private $timeout: ng.ITimeoutService,
            private ngMap,
            private trafficDataService: Services.ITrafficDataService
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
        this.trafficDataService.getTrafficMeters().then(
            (response) => {
                this.meters = response.data;
                var ids = Enumerable.From(this.meters).Select((x) => x.id).ToArray();
                var list = { meterIds: ids };
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
                var congestion = this.meters[i].congestion;
                var latlng = { lat: this.meters[i].latitude, lng: this.meters[i].longitude};
                var request = {
                    origin: latlng,
                    destination: latlng,
                    travelMode: google.maps.TravelMode.DRIVING
                };

                directionsService.route(request, (response, status) => {
                    if (status == google.maps.DirectionsStatus.OK) {
                        var pos = response.routes[0].legs[0].start_location;
                        this.gMarkers.push({ lat: pos.lat(), long: pos.lng() });

                        this.$scope.$apply();

                        if (this.gMarkers.length >= 2) {
                            this.displayRouteForMarkers();
                        }
                    }    
                });
            }
        }

        displayRouteForMarkers() {
            var directionsService = new google.maps.DirectionsService();
            var start = { lat: this.meters[0].latitude, lng: this.meters[0].longitude };
            var end = { lat: this.meters[1].latitude, lng: this.meters[1].longitude };

            var request = {
                origin: start,
                destination: end,
                travelMode: google.maps.TravelMode.DRIVING
            }

            directionsService.route(request, (response, status) => {
                if (status == google.maps.DirectionsStatus.OK) {
                    var route = response;
                    var snappedPolyline = new google.maps.Polyline({
                        path: route.routes[0].overview_path,
                        strokeColor: '#009933',
                        strokeWeight: 7,
                        strokeOpacity: 0.5
                    });
                    snappedPolyline.setMap(this.map);
                    this.polyLines.push(snappedPolyline);
                }
            });
        }

        showPopup = (evt, index) => {
            var meter = this.meters[index];
            var id = 'meter-' + meter.id;

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
            //Remove Padded Backgrounds
            var iwOuter = $('.gm-style-iw');
            var iwBackground = iwOuter.prev();
            iwBackground.children(':nth-child(2)').css({ 'display': 'none' });
            iwBackground.children(':nth-child(4)').css({ 'display': 'none' });

            //Reposition Window
            iwOuter.parent().parent().css({ left: '20px' });

            //Move the Arrow
            iwBackground.children(':nth-child(1)').attr('style', function (i, s) { return s + 'left: 145px !important;' });
            iwBackground.children(':nth-child(3)').attr('style', function (i, s) { return s + 'left: 145px !important;' });
            iwBackground.children(':nth-child(3)').find('div').children().css({ 'box-shadow': 'rgba(156, 156, 156, 0.6) 0px 1px 6px', 'z-index': '1' });

            var iwCloseBtn = iwOuter.next();
            iwCloseBtn.css({
                opacity: '1',
                right: '52px',
                top: '17px'
            });
            iwCloseBtn.mouseout(function () {
                $(this).css({ opacity: '1' });
            });

            this.adjustPopupPosition();
        }

        adjustPopupPosition() {
            if ($('.ng-map-info-window').length) {
                var currentTop = $('.ng-map-info-window').position().top;
                var newTop = (currentTop - 40) + 'px';
                $('.ng-map-info-window').css('top', newTop);
            }
        }
    }
}