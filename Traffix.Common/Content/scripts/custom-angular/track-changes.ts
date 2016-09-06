module NTA.Common.Directives {
    export class TrackChanges implements ng.IDirective {
        restrict = 'A';

        constructor(private $rootScope: ng.IRootScopeService, private $uibModal, private $state) {
        }

        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) => {
            //grab form name:
            var formName = $(element).attr('name');
            var formObject = scope[formName];

            this.$rootScope.$on('$stateChangeStart', (event, toState, toParams) => {
                if (formObject.$dirty) {
                    event.preventDefault();

                    function changesController($scope, uibModalInstance, $state) {
                        $scope.ok = () => {
                            uibModalInstance.close();
                            formObject.$dirty = false;
                            $state.go(toState, toParams);
                        };
                        $scope.cancel = () => {
                            uibModalInstance.close();
                        };
                    }
                    changesController.$inject = ['$scope', '$uibModalInstance', '$state']; //for minification


                    this.$uibModal.open({
                        template:
                        '<div class="modal-header"><h3 class="modal-title">Unsaved changes</h3></div>' +
                        '<div class="modal-body">Are you sure you want to navigate away without saving?</div>' +
                        '<div class="modal-footer">' +
                        '<button class="btn btn-primary" ng-click= "ok()">OK</button>' +
                        '<button class="btn btn-default" ng-click="cancel()">Cancel</button>' +
                        '</div >',
                        controller: changesController,
                        backdrop: 'static',
                        keyboard: false,
                        size: ''
                    });
                }
            });
        }

        static factory(): ng.IDirectiveFactory {
            var directive: ($rootScope: ng.IRootScopeService, $modal: any, $state: any) => TrackChanges = ($rootScope: ng.IRootScopeService, $modal, $state) => new TrackChanges($rootScope, $modal, $state);
            directive.$inject = ['$rootScope', '$uibModal', '$state'];
            return directive;
        }
    }
}