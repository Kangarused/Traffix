module NTA.Common.Directives {
    export class IncludeReplace implements ng.IDirective {
        require: 'ngInclude';
        restrict = 'A';

        constructor() {
        }

        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) => {
            element.replaceWith(element.children());
        }

        static factory(): ng.IDirectiveFactory {
            var directive: () => IncludeReplace = () => new IncludeReplace();
            return directive;
        }
    }
}