/// <reference path="angularCommonDefinitions.d.ts" />
angularApplication.config([
    'datetimepickerProvider', function (datetimepickerProvider) {
        datetimepickerProvider.setOptions({
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar-o',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down',
                previous: 'fa fa-chevron-left',
                next: 'fa fa-chevron-right',
                today: 'fa fa-crosshairs',
                clear: 'fa fa-trash-0',
                close: 'fa fa-times'
            },
            format: 'DD/MM/YYYY h:mm'
        });
    }
]);
angularApplication.config([
    'datepickerProvider', function (datetimepickerProvider) {
        datetimepickerProvider.setOptions({
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar-o',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down',
                previous: 'fa fa-chevron-left',
                next: 'fa fa-chevron-right',
                today: 'fa fa-crosshairs',
                clear: 'fa fa-trash-0',
                close: 'fa fa-times'
            },
            format: 'DD/MM/YYYY'
        });
    }
])
    .config([
    '$validationProvider', function ($validationProvider) {
        $validationProvider.showSuccessMessage = false;
        $validationProvider.showErrorMessage = true;
        $validationProvider.setErrorHTML(function (msg) { return "<span class=\"help-block\">" + msg + "</span>"; });
        $validationProvider.validCallback = function (element) {
            $(element).parents('.form-group:first').removeClass('has-error');
        };
        $validationProvider.invalidCallback = function (element) {
            $(element).parents('.form-group:first').addClass('has-error');
        };
    }
])
    .run([
    '$templateCache', function ($templateCache) {
        var template = '<nav>' +
            '<ul class="pagination pagination-sm">' +
            '<li ng-class="{disabled: currentPage === 1}" class="pagination-first"><a href ng-click="selectPage(1)">First</a></li>' +
            '<li ng-class="{disabled: currentPage === 1}" class="pagination-prev"><a href ng-click="selectPage(currentPage - 1)">Previous</a></li>' +
            '<li ng-repeat="page in pages track by $index" ng-class="{active: page === currentPage}" class="pagination-page"><a href ng-click="selectPage(page)">{{page}}</a></li>' +
            '<li ng-class="{disabled: currentPage === pages.length}" class="pagination-next"><a href ng-click="selectPage(currentPage + 1)">Next</a></li>' +
            '<li ng-class="{disabled: currentPage === pages.length}" class="pagination-last" > <a href ng-click="selectPage(pages.length)">Last</a></li>' +
            '</ul>' +
            '</nav>';
        $templateCache.put('template/smart-table/pagination.html', template);
    }
])
    .run([
    '$state', '$rootScope', function ($state, $rootScope) {
        $rootScope.$state = $state;
    }
]);
//# sourceMappingURL=angularCommonInitSettings.js.map