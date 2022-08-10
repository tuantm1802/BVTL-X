angular.module('ui.combotree', [])
    .directive('uiComboTree', [function () {
        return {
            restrict: "EAC",
            replace: true,
            scope: {
                comboTreeInit: "=",
                multiple: "=",
                rootItem: "=",
                onCallback: "&"
            },
            link: function (scope, elem, attrs, $parse) {
                var __treeSource = [];
                if (scope.comboTreeInit.data !== null && scope.comboTreeInit.data.length > 0) {
                    var __treeItem = scope.comboTreeInit.data.filter(x => x.ParentId === 0);
                    if (__treeItem !== null && __treeItem.length > 0) {
                        for (var i = 0; i < __treeItem.length; i++) {
                            if(scope.rootItem !== null && scope.rootItem !== undefined){
                                __itemComboTree = { id: scope.rootItem.id, title: scope.rootItem.title };
                            }
                            else{
                                var __itemComboTree = { id: __treeItem[i].Id, title: __treeItem[i].Name };
                            }
                            // var __itemComboTree = { id: 0, title: 'Lực lượng' };
                            // kiểm tra có con không
                            var __itemChilds = scope.comboTreeInit.data.filter(x => x.ParentId === __treeItem[i].Id);

                            if (__itemChilds !== null && __itemChilds.length > 0) {
                                GetChild(__itemComboTree, scope.comboTreeInit.data, __treeItem[i].Id);
                            }
                            __treeSource.push(__itemComboTree);
                        }
                    }
                }

                function GetChild(_listTree, _data, _id) {
                    var _itemTree = _data.filter(x => x.ParentId === _id);

                    if (_itemTree !== null && _itemTree.length > 0) {
                        _listTree.subs = [];
                        for (var i = 0; i < _itemTree.length; i++) {
                            var __itemData =
                            {
                                id: _itemTree[i].Id,
                                title: _itemTree[i].Name
                            };
                            var __childs = _data.filter(x => x.ParentId === _itemTree[i].Id);
                            if (__childs !== null && __childs.length > 0) {
                                GetChild(__itemData, _data, _itemTree[i].Id);
                            }
                            _listTree.subs.push(__itemData);
                        }

                    }
                }

                var comboTree = $(elem[0]).comboTree({
                    source: __treeSource,
                    isMultiple: false
                });
                
                scope.onCallback({ data: comboTree });
                scope.$watch('ngChange', function (val) {
                })
            }
        };
    }]);