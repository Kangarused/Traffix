/// <reference path="angularCommonDefinitions.d.ts" />
angularApplication.config([
    'datetimepickerProvider', datetimepickerProvider => {
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
        'datepickerProvider', datetimepickerProvider => {
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
        '$validationProvider', $validationProvider => {
            $validationProvider.showSuccessMessage = false;
            $validationProvider.showErrorMessage = true;
            $validationProvider.setErrorHTML(msg => "<span class=\"help-block\">" + msg + "</span>");

            $validationProvider.validCallback = (element) => {
                $(element).parents('.form-group:first').removeClass('has-error');
            };

            $validationProvider.invalidCallback = (element) => {
                $(element).parents('.form-group:first').addClass('has-error');
            };
        }
    ])
    .run([
        '$templateCache', ($templateCache) => {
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
        '$state', '$rootScope', ($state, $rootScope) => {
            $rootScope.$state = $state;
        }
    ]);

