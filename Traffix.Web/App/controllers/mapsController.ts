module Traffix.Controllers {
    export class MapsController {
        static $inject = ['$scope', '$window', '$rootScope', '$timeout', 'NgMap', 'trafficDataService', 'colorService'];
        googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCmq2yT3zYiQ1R6uWcP5mB_R_8WnZeJySQ&libraries=places&callback=initMap";
        defaultLat = -12.4463818;
        defaultLong = 130.9157756;
        defaultZoom = 3;
        headerHeight = 48;
        traffixHub = null;
        traffixProxy = null;
        directionsService;
        map;
        searchBox;
        height: number;
        polyLines = [];

        allMeters: Models.IAnimatedTrafficMeter[] = [];
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

            //Setup web-socket connection using Signalr
            this.traffixHub = $.hubConnection();
            this.traffixHub.logging = true;
            this.traffixProxy = this.traffixHub.createHubProxy('trafficHub');
            this.traffixProxy.logging = true;
            this.traffixProxy.on('meterUpdated', (event, meter, log) => {
                //Converting from uppercase notation to lowercase notation due to signalr passing the C# class
                var convertMeter = { id: meter.Id, name: meter.Name, congestion: meter.Congestion };
                var convertLog = {
                    id: log.Id,
                    meterId: log.MeterId,
                    time: log.Time,
                    speed: log.Speed
                };
                this.updateMeter(convertMeter, convertLog);
            });
            this.traffixHub.start();
        }

        updateMeter(meter, log) {
            //Get meter from list 
            var curMeter = Enumerable.From(this.allMeters).FirstOrDefault(null, x => x.id === meter.id);
            if (curMeter != null) {
                //Get array index of meter
                var meterIndex = this.allMeters.indexOf(curMeter);

                //Update the congestion of the meter used to render the marker and set bounce animation
                this.allMeters[meterIndex].congestion = meter.congestion;
                this.allMeters[meterIndex].animation = "Animation.BOUNCE";
                this.$scope.$apply();

                //After animation runs one time, remove the animation attribute (Animation length ~750ms)
                this.$timeout(() => {
                    this.allMeters[meterIndex].animation = " ";
                    this.$scope.$apply();
                }, 750);

                //Get current log from array 
                var curLog = Enumerable.From(this.logs).FirstOrDefault(null, x => x.meterId === meter.id);
                if (curLog != null) {
                    //Get array index of current log and push new log data into log list
                    var logIndex = this.logs.indexOf(curLog);
                    this.logs[logIndex].logs.push(log);
                }
            }

            //Update the popup information if the popup is being viewed for the updated meter
            if (this.popupInfo.name === meter.name) {
                this.updatePopupInfo(meter);
                this.$scope.$apply();
            }
        }

        initMap(map) {
            this.map = map;
            this.directionsService = new google.maps.DirectionsService();
            this.loadTrafficData("Northern Territory");

            //Prevent the map from being zoomed out too much and adjust the popup position
            this.map.addListener('zoom_changed', () => {
                if (this.map.getZoom() < 9) {
                    this.map.setZoom(9);
                }
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
                    var region = null;
                    for (var i = 0; i < place.address_components.length; i++) {
                        //Gets the full name of the state/territory at the searched location
                        if (place.address_components[i].types[0] == 'administrative_area_level_1') {
                            region = place.address_components[i].long_name;
                        }
                    }
                    //Begin the map update process
                    this.updateMap(region);
                    //Get the bounds of the new location
                    if (place.geometry.viewport) {
                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                //Set the map to the new bounds location ie. instantly jump to the new location
                this.map.fitBounds(bounds);
            });
        }

        updateMap(region) {
            //Remove all markers and polylines from the map and load the data for the specified region
            if (region != null) {
                for (var i = 0; i < this.polyLines.length; i++) {
                    this.polyLines[i].setMap(null);
                }
                this.polyLines = [];
                this.allMeters.length = 0;
                this.meters.length = 0;
                this.logs.length = 0;
                this.loadTrafficData(region);
            }
        }

        loadTrafficData(region) {
            this.trafficDataService.getLinkedTrafficMeters(region).then(
            (response) => {
                this.meters = response.data;
                var list = {meterIds: []};
                for (var i = 0; i < this.meters.length; i++) {
                    for (var x = 0; x < this.meters[i].linkedMeters.length; x++) {
                        //Combine all meters from all groups into one single list for use with the marker repeater
                        this.allMeters.push({
                            animation: " ",
                            id: this.meters[i].linkedMeters[x].id,
                            name: this.meters[i].linkedMeters[x].name,
                            region: this.meters[i].linkedMeters[x].region,
                            latitude: this.meters[i].linkedMeters[x].latitude,
                            longitude: this.meters[i].linkedMeters[x].longitude,
                            congestion: this.meters[i].linkedMeters[x].congestion,
                            dateActive: this.meters[i].linkedMeters[x].dateActive,
                            linkId: this.meters[i].linkedMeters[x].linkId
                        });
                        //Get a list of meter ids grouped by link id -- avoids passing the entire data array around
                        list.meterIds.push(this.meters[i].linkedMeters[x].id);
                    }
                }
                //Get traffic log data for the list of meters
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
                        this.displayRouteForMarkers(0, this.meters[0].linkedMeters[0], this.meters[0].linkedMeters[this.meters[0].linkedMeters.length - 1]);
                    }
                }
            }
        }

        displayRouteForMarkers(group, start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
            if (this.meters[group].linkedMeters.length > 1) {
                
                var startPoint = { lat: start.latitude, lng: start.longitude };
                var endPoint = { lat: end.latitude, lng: end.longitude };
                var wayPoints = [];
                var request;

                if (this.meters[group].linkedMeters.length > 2) {
                    for (var i = 1; i < this.meters[group].linkedMeters.length - 1; i++) {
                        wayPoints.push({
                            location: { lat: this.meters[group].linkedMeters[i].latitude, lng: this.meters[group].linkedMeters[i].longitude },
                            stopover: true
                        });
                    }
                    request = {
                        origin: startPoint,
                        destination: endPoint,
                        waypoints: wayPoints,
                        travelMode: google.maps.TravelMode.DRIVING
                    }
                    this.directionsService.route(request, this.loadRoutes(group, true, start, end));
                } else {
                    request = {
                        origin: startPoint,
                        destination: endPoint,
                        travelMode: google.maps.TravelMode.DRIVING
                    }
                    this.directionsService.route(request, this.loadRoutes(group, false, start, end));
                }
            } else {
                if (group < this.meters.length) {
                    group++;
                    this.displayRouteForMarkers(group, this.meters[group].linkedMeters[0], this.meters[group].linkedMeters[1]);
                }
            }
        }

        loadRoutes(group, hasWayPoints: boolean, start: Models.ITrafficMeter, end: Models.ITrafficMeter) {
            return (response, status) => {
                if (status === google.maps.DirectionsStatus.OK) {
                    var route = response;
                    if (hasWayPoints) {
                        this.drawWaypointRoutes(route, group);
                    } else {
                        this.drawSingleRoute(route, group);
                    }
                }
            }
        }

        drawWaypointRoutes(route, group) {
            var points = route.routes[0].legs.length;
            var currentMeter = 1;
            for (var i = 0; i < points; i++) {
                var pathColor = this.colorService.RGB2Hex(this.getCongestionColor(this.meters[group].linkedMeters[currentMeter-1].congestion));
                var currentColor = this.colorService.RGBtoHSV(this.getCongestionColor(this.meters[group].linkedMeters[currentMeter - 1].congestion));
                var endColor = this.colorService.RGBtoHSV(this.getCongestionColor(this.meters[group].linkedMeters[currentMeter].congestion));

                var steps = route.routes[0].legs[i].steps[0].path.length;
                var increment = this.colorService.calculateIncrement(currentColor, endColor, steps);
                this.drawPath(route.routes[0].legs[i].steps[0].path, steps, increment, pathColor, currentColor, endColor);
                currentMeter++;
            }
            this.loadNextRoute(group);
        }

        drawSingleRoute(route, group) {
            var pathColor = this.colorService.RGB2Hex(this.getCongestionColor(this.meters[group].linkedMeters[0].congestion));
            var currentColor = this.colorService.RGBtoHSV(this.getCongestionColor(this.meters[group].linkedMeters[0].congestion));
            var endColor = this.colorService.RGBtoHSV(this.getCongestionColor(this.meters[group].linkedMeters[1].congestion));

            var steps = route.routes[0].overview_path.length;
            var increment = this.colorService.calculateIncrement(currentColor, endColor, steps);
            this.drawPath(route.routes[0].overview_path, steps, increment, pathColor, currentColor, endColor).then(
            (resposne) => {
                this.loadNextRoute(group);
            });
        }

        drawPath(pathComponents, steps, increment, pathColor, currentColor, endColor) {
            return new Promise((resolve, reject) => {
                if (pathComponents != null) {
                    for (var i = 0; i < steps; i++) {
                        var path;
                        if (i + 1 >= steps) {
                            path = [pathComponents[i], pathComponents[i]];
                        } else {
                            path = [pathComponents[i], pathComponents[i + 1]];
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
                        resolve();
                    }
                } else {
                    reject(Error("Path Components is Undefined"));
                }
            });
        }

        loadNextRoute(group: number) {
            group++;
            if (group < this.meters.length) {
                //Using timeout to avoid breaching google request limit
                this.$timeout(() => {
                    this.displayRouteForMarkers(group, this.meters[group].linkedMeters[0], this.meters[group].linkedMeters[this.meters[group].linkedMeters.length - 1]);
                }, 1000);
            }
        }

        getCongestionColor(congestion) {
            if (congestion === 1) {
                return this.colors.medium;
            } else if (congestion === 2) {
                return this.colors.high;
            }
            return this.colors.low;
        }

        showPopup = (evt, meter: Models.ITrafficMeter) => {
            var id = 'meter-' + meter.id;
            this.updatePopupInfo(meter);
            this.map.customMarkers.customPopup.setVisible(true);
            this.map.customMarkers.customPopup.setPosition(this.map.markers[id].getPosition());
        }

        hidePopup = (evt) => {
            this.map.customMarkers.customPopup.setVisible(false);
        }

        updatePopupInfo(meter) {
            this.popupInfo.name = meter.name;
            this.popupInfo.congestion = meter.congestion;
            this.popupInfo.header = this.getCongestionHeader(meter.congestion);
            this.popupInfo.avgSpeed = this.getAverageSpeed(meter);
            this.popupInfo.totalVehicles = this.getTotalVehicles(meter);
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

        getAverageSpeed(meter) {
            var meterLogs = Enumerable.From(this.logs).FirstOrDefault(null, (x) => x.meterId === meter.id);
            if (meterLogs != null) {
                var avgSpeed = 0;
                for (var i = 0; i < meterLogs.logs.length; i++) {
                    avgSpeed += meterLogs.logs[i].speed;
                }
                avgSpeed = +((avgSpeed / meterLogs.logs.length).toFixed(2));
                return avgSpeed;
            }
        }

        getTotalVehicles(meter) {
            var meterLogs = Enumerable.From(this.logs).FirstOrDefault(null, (x) => x.meterId === meter.id);
            if (meterLogs != null) {
                var totalVehicles = meterLogs.logs.length;
                return totalVehicles;  
            }
        }

        getMarkerIcon(congestion: number) {
            if (congestion === 0) {
                return '../../Content/images/markers/greenMarker.png';
            }else if (congestion === 1) {
                return '../../Content/images/markers/orangeMarker.png';
            }else {
                return '../../Content/images/markers/redMarker.png';
            }
        }
    }
}