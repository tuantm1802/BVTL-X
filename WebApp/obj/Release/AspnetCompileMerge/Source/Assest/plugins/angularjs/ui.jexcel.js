angular.module('ui.jexcel', [])
    .directive('uiJexel', [function () {
        return {
            require: "ngModel",
            restrict: "EAC",
            replace: true,
            scope: {
                jexcelInit: "=",
                callback: "&"
            },
            link: function (scope, elem, attrs, $parse) {
                var table = jexcel(elem[0], {
                    data: scope.jexcelInit.data,
                    columns: scope.jexcelInit.column,
                    tableOverflow: true,
                    tableWidth: '100%',
                    footers: scope.jexcelInit.footers,
                    onchange: scope.jexcelInit.changed,
                    onbeforechange: scope.jexcelInit.beforeChange,
                    oninsertrow: scope.jexcelInit.insertedRow,
                    oninsertcolumn: scope.jexcelInit.insertedColumn,
                    ondeleterow: scope.jexcelInit.deletedRow,
                    ondeletecolumn: scope.jexcelInit.deletedColumn,
                    onselection: scope.jexcelInit.selectionActive,
                    onsort: scope.jexcelInit.sort,
                    onresizerow: scope.jexcelInit.resizeRow,
                    onresizecolumn: scope.jexcelInit.resizeColumn,
                    onmoverow: scope.jexcelInit.moveRow,
                    onmovecolumn: scope.jexcelInit.moveColumn,
                    onload: scope.jexcelInit.loaded,
                    onblur: scope.jexcelInit.blur,
                    onfocus: scope.jexcelInit.focus,
                    onpaste: scope.jexcelInit.paste,
                    pagination: scope.jexcelInit.pagination,
                    nestedHeaders: scope.jexcelInit.nestedHeaders,
                    updateTable: scope.jexcelInit.updateTable,
                    text: scope.jexcelInit.text
                });
                scope.callback({ data: table });
            }
        };
    }]);