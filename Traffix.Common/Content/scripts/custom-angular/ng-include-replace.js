var NTA;
(function (NTA) {
    var Common;
    (function (Common) {
        var Directives;
        (function (Directives) {
            var IncludeReplace = (function () {
                function IncludeReplace() {
                    this.restrict = 'A';
                    this.link = function (scope, element, attrs, ctrl) {
                        element.replaceWith(element.children());
                    };
                }
                IncludeReplace.factory = function () {
                    var directive = function () { return new IncludeReplace(); };
                    return directive;
                };
                return IncludeReplace;
            }());
            Directives.IncludeReplace = IncludeReplace;
        })(Directives = Common.Directives || (Common.Directives = {}));
    })(Common = NTA.Common || (NTA.Common = {}));
})(NTA || (NTA = {}));
//# sourceMappingURL=ng-include-replace.js.map