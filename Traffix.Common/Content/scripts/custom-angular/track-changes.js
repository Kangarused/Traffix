var NTA;
(function (NTA) {
    var Common;
    (function (Common) {
        var Directives;
        (function (Directives) {
            var TrackChanges = (function () {
                function TrackChanges($rootScope, $uibModal, $state) {
                    var _this = this;
                    this.$rootScope = $rootScope;
                    this.$uibModal = $uibModal;
                    this.$state = $state;
                    this.restrict = 'A';
                    this.link = function (scope, element, attrs, ctrl) {
                        //grab form name:
                        var formName = $(element).attr('name');
                        var formObject = scope[formName];
                        _this.$rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
                            if (formObject.$dirty) {
                                event.preventDefault();
                                function changesController($scope, uibModalInstance, $state) {
                                    $scope.ok = function () {
                                        uibModalInstance.close();
                                        formObject.$dirty = false;
                                        $state.go(toState, toParams);
                                    };
                                    $scope.cancel = function () {
                                        uibModalInstance.close();
                                    };
                                }
                                changesController.$inject = ['$scope', '$uibModalInstance', '$state']; //for minification
                                _this.$uibModal.open({
                                    template: '<div class="modal-header"><h3 class="modal-title">Unsaved changes</h3></div>' +
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
                    };
                }
                TrackChanges.factory = function () {
                    var directive = function ($rootScope, $modal, $state) { return new TrackChanges($rootScope, $modal, $state); };
                    directive.$inject = ['$rootScope', '$uibModal', '$state'];
                    return directive;
                };
                return TrackChanges;
            }());
            Directives.TrackChanges = TrackChanges;
        })(Directives = Common.Directives || (Common.Directives = {}));
    })(Common = NTA.Common || (NTA.Common = {}));
})(NTA || (NTA = {}));
//# sourceMappingURL=track-changes.js.map