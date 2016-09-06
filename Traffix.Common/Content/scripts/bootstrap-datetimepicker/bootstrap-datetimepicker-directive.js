'use strict';
angular
    .module('datetimepicker', [])
    .provider('datetimepicker', function () {
        var default_options = {};

        this.setOptions = function (options) {
            default_options = options;
        };

        this.$get = function () {
            return {
                getOptions: function () {
                    return default_options;
                }
            };
        };
    })
    .provider('datepicker', function () {
        var default_options = {};

        this.setOptions = function (options) {
            default_options = options;
        };

        this.$get = function () {
            return {
                getOptions: function () {
                    return default_options;
                }
            };
        };
    })
    .factory('datetimepickerfactory', ['$timeout', function($timeout) {
        return {
            createInstance: function(defaultOptions) {
                return {
                    require: '?ngModel',
                    restrict: 'AE',
                    link: function ($scope, $element, $attrs, ngModelCtrl) {
                        var options = defaultOptions;

                        $element.on('dp.change', function (e) {
                            if (ngModelCtrl) {
                                $timeout(function () {
                                    var isoValue = $element.data('DateTimePicker').date() ?
                                        $element.data('DateTimePicker').date().format("YYYY-MM-DDTHH:mm:ssZ") : null;
                                    if (isoValue != ngModelCtrl.$viewValue) {
                                        ngModelCtrl.$setViewValue(isoValue);
                                    }
                                });
                            }
                        }).datetimepicker(options);

                        function setPickerValue() {
                            var date = options.defaultDate || null;

                            if (ngModelCtrl && ngModelCtrl.$viewValue) {
                                date = ngModelCtrl.$viewValue;
                            }
                            if (date != null) {
                                $element
                                    .data('DateTimePicker')
                                    .date(new Date(date));
                            }
                        }

                        if (ngModelCtrl) {
                            ngModelCtrl.$render = function () {
                                setPickerValue();
                            };
                        }

                        setPickerValue();
                    }
                };
            }
        }
    }])
    .directive('datetimepicker', ['$timeout', 'datetimepicker', 'datetimepickerfactory', function ($timeout, datetimepicker, datetimepickerfactory) {
        return datetimepickerfactory.createInstance(datetimepicker.getOptions());
    }
    ])
    .directive('datepicker', ['$timeout', 'datepicker', 'datetimepickerfactory', function ($timeout, datepicker, datetimepickerfactory) {
        return datetimepickerfactory.createInstance(datepicker.getOptions());
    }]);