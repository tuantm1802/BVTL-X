angular.module('ui.treetable', [])
    .directive('uiTreeTable', [function () {
        return {
            restrict: "EAC",
            replace: true,
            templateUrl: '/app/directives/treeTableDirective.html',
            scope: {
                showCheckbox: "@",
                listColumnTreetable: "@",
                treeData: "@",
                eventClickRow: "&",
                eventChangeCheckbox: "&"
            },
            link: function (scope, elem, attrs, $parse) {
                scope.ClickTreeTableRow = function (item) {
                    if (scope.eventClickRow) {
                        scope.eventClickRow({ item: item });
                    }
                };
                scope.ChangeTreeTableCheckbox = function (item) {
                    if (scope.eventChangeCheckbox) {
                        scope.eventChangeCheckbox({ item: item });
                    }
                };
                //scope.callback({ data: table });
            }
        };
    }]);