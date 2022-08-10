var app = angular.module("web-app", ['ui.bootstrap', 'cp.ngConfirm', 'ui.select2', 'ui.jexcel', 'ngValidate', 'treeGrid', 'treeGridqs', 'ui.combotree']).run(function () {
    Array.prototype.sum = function (prop) {
        var total = 0
        for (var i = 0, _len = this.length; i < _len; i++) {
            total += parseInt(this[i][prop])
        }
        return total
    }
});

app.run(['$rootScope', function ($rootScope) {

    $rootScope.AppCodeP1 = 'KE_HOACH_TH';
    $rootScope.AppCodeP3 = 'PHUONG_TIEN_BO';
    $rootScope.AppCodeP4 = 'PHUONG_TIEN_THUY';
    $rootScope.AppCodeP5 = 'VTU_THIET_BI';
    $rootScope.AppCodeP6 = 'VU_KHI_VL_NO';
    $rootScope.AppCodeP7 = 'CP_QUAN_TRANG';
    $rootScope.AppCodeP8 = 'KHO_HH';
    $rootScope.AppCodeP9 = 'VAN_TAI';
    $rootScope.AppCodeP10 = 'TAI_CHINH';
    $rootScope.AppCodeTTDTMS = 'TT_DT_MS';
}]);
app.constant("constant", (function () {
    return {
        jexcelConfig: {
            noRecordsFound: '', /*'Không có dữ liệu',*/
            showingPage: '',/*'Hiển thị {0} của {1} bản ghi',*/
            show: 'Hiển thị ',
            search: 'TÌm kiếm',
            entries: ' bản ghi',
            columnName: 'Tên cột',
            insertANewColumnBefore: 'Thêm mới vào trước một cột',
            insertANewColumnAfter: 'Thêm mới vào sau một cột',
            deleteSelectedColumns: 'Xóa các cột được chọn',
            renameThisColumn: 'Đổi tên cột',
            orderAscending: 'Sắp xếp tăng dần',
            orderDescending: 'Sắp xếp giảm dần',
            insertANewRowBefore: 'Thêm mới vào trước một dòng',
            insertANewRowAfter: 'Thêm mới vào sau một dòng',
            deleteSelectedRows: 'Xóa các dòng được chọn',
            editComments: 'Sửa ghi chú',
            addComments: 'Thêm ghi chú',
            comments: 'Ghi chú',
            clearComments: 'Xó ghi chú',
            copy: 'Sao chép',
            paste: 'Dán',
            saveAs: 'Lưu dưới dạng',
            about: 'Thông tin',
            areYouSureToDeleteTheSelectedRows: 'Bạn có chắc chắn xóa các dòng được chọn?',
            areYouSureToDeleteTheSelectedColumns: 'Bạn có chắc chắn xóa các cột được chọn?',
            thisActionWillDestroyAnyExistingMergedCellsAreYouSure: 'Bạn có chắc xóa toàn bộ các dữ liệu đã gộp?',
            thisActionWillClearYourSearchResultsAreYouSure: 'Bạn có chắc xóa toàn boọ dữ liệu tìm kiếm?',
            thereIsAConflictWithAnotherMergedCell: 'Dữ liệu đã gộp ở ô khác',
            invalidMergeProperties: '',
            cellAlreadyMerged: 'Ô đã được gộp',
            noCellsSelected: 'Không có ô nào được chọn',
        }
    }
})());
app.directive("select2", function ($timeout, $parse) {
    return {
        restrict: 'AC',
        require: 'ngModel',
        link: function (scope, element, attrs) {
            $timeout(function () {
                element.select2();
                element.select2Initialized = true;
            });

            var refreshSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.trigger('change');
                });
            };

            var recreateSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.select2('destroy');
                    element.select2();
                });
            };

            scope.$watch(attrs.ngModel, refreshSelect);

            if (attrs.ngOptions) {
                var list = attrs.ngOptions.match(/ in ([^ ]*)/)[1];
                // watch for option list change
                scope.$watch(list, recreateSelect);
            }

            if (attrs.ngDisabled) {
                scope.$watch(attrs.ngDisabled, refreshSelect);
            }
        }
    };
});
app.filter('trustUrl', function ($sce) {
    return function (url) {
        return $sce.trustAsResourceUrl(url);
    };
});
app.filter('unsafe', function ($sce) {
    return function (url) {
        return $sce.trustAsHtml(url);
    };
});
app.factory('showToast', ['$window', function (win) {
    return function showToast() {
        var title = "Vui lòng đợi trong giây lát";
        var icon = "loading";
        var duration = $("#duration").val() * 1;
        if ($('.toast-wrap .loading').length==0)
            $.Toast.showToast({ title: title, duration: duration, icon: icon, image: '' });
    }
}]);
app.factory('chuyenGiaSangChu', ['$window', function (value) {
    var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
    var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");

    //1. Hàm đọc số có ba chữ số;
    function DocSo3ChuSo(baso) {
        var tram;
        var chuc;
        var donvi;
        var KetQua = "";
        tram = parseInt(baso / 100);
        chuc = parseInt((baso % 100) / 10);
        donvi = baso % 10;
        if (tram == 0 && chuc == 0 && donvi == 0) return "";
        if (tram != 0) {
            KetQua += ChuSo[tram] + " trăm ";
            if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
        }
        if ((chuc != 0) && (chuc != 1)) {
            KetQua += ChuSo[chuc] + " mươi";
            if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
        }
        if (chuc == 1) KetQua += " mười ";
        switch (donvi) {
            case 1:
                if ((chuc != 0) && (chuc != 1)) {
                    KetQua += " mốt ";
                }
                else {
                    KetQua += ChuSo[donvi];
                }
                break;
            case 5:
                if (chuc == 0) {
                    KetQua += ChuSo[donvi];
                }
                else {
                    KetQua += " lăm ";
                }
                break;
            default:
                if (donvi != 0) {
                    KetQua += ChuSo[donvi];
                }
                break;
        }
        return KetQua;
    }

    //2. Hàm đọc số thành chữ (Sử dụng hàm đọc số có ba chữ số)
    return function DocTienBangChu(SoTien) {
        var lan = 0;
        var i = 0;
        var so = 0;
        var KetQua = "";
        var tmp = "";
        var ViTri = new Array();
        if (SoTien < 0) return "Số tiền âm !";
        if (SoTien == 0) return "Không đồng !";
        if (SoTien > 0) {
            so = SoTien;
        }
        else {
            so = -SoTien;
        }
        if (SoTien > 8999999999999999) {
            //SoTien = 0;
            return "Số quá lớn!";
        }
        ViTri[5] = Math.floor(so / 1000000000000000);
        if (isNaN(ViTri[5]))
            ViTri[5] = "0";
        so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
        ViTri[4] = Math.floor(so / 1000000000000);
        if (isNaN(ViTri[4]))
            ViTri[4] = "0";
        so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
        ViTri[3] = Math.floor(so / 1000000000);
        if (isNaN(ViTri[3]))
            ViTri[3] = "0";
        so = so - parseFloat(ViTri[3].toString()) * 1000000000;
        ViTri[2] = parseInt(so / 1000000);
        if (isNaN(ViTri[2]))
            ViTri[2] = "0";
        ViTri[1] = parseInt((so % 1000000) / 1000);
        if (isNaN(ViTri[1]))
            ViTri[1] = "0";
        ViTri[0] = parseInt(so % 1000);
        if (isNaN(ViTri[0]))
            ViTri[0] = "0";
        if (ViTri[5] > 0) {
            lan = 5;
        }
        else if (ViTri[4] > 0) {
            lan = 4;
        }
        else if (ViTri[3] > 0) {
            lan = 3;
        }
        else if (ViTri[2] > 0) {
            lan = 2;
        }
        else if (ViTri[1] > 0) {
            lan = 1;
        }
        else {
            lan = 0;
        }
        for (i = lan; i >= 0; i--) {
            tmp = DocSo3ChuSo(ViTri[i]);
            KetQua += tmp;
            if (ViTri[i] > 0) KetQua += Tien[i];
            if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
        }
        if (KetQua.substring(KetQua.length - 1) == ',') {
            KetQua = KetQua.substring(0, KetQua.length - 1);
        }
        KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
        return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
    };
}]);
app.factory('hideLoading', ['$window', function (win) {
    return function hideLoading() {
        $.Toast.hideToast();
    }
}]);
app.directive('uiDatepicker', function () {
    return {
        require: 'ngModel',
        restrict: 'EAC',
        replace: true,
        scope: {

        },
        link: function (scope, elem, attrs) {
            $(elem[0]).datepicker({
                autoclose: true,
                todayHighlight: true
            })
                //show datepicker when clicking on the icon
                .next().on(ace.click_event, function () {
                    $(this).prev().focus();
                });
        }
    };
})
app.directive('uiChoseFile', function () {
    return {
        require: '',
        restrict: 'EAC',
        replace: true,
        scope: {

        },
        link: function (scope, elem, attrs) {
            $(elem[0]).ace_file_input({
                no_file: 'Không có ...',
                btn_choose: 'Chọn tệp',
                btn_change: 'Thay đổi',
                droppable: false,
                onchange: null,
                thumbnail: false
            });
        }
    };
})
app.directive('customzdatetime', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            console.log(attrs.type);
            element.datetimepicker({
                useCurrent: false,
                locale: 'vi',
                // debug: true,
                widgetPositioning: {
                    horizontal: attrs.horizontal === undefined ? 'auto' : attrs.horizontal,
                    vertical: 'bottom'
                },
                format: attrs.formatdate === undefined ? 'DD/MM/YYYY' : attrs.formatdate,
            }).on('dp.change', function (e) {

                // var date = (e.date._d.getDate() - 2) + '-' + (e.date._d.getMonth() + 1) + '-' + e.date._d.getFullYear();
                var date = moment(e.date._d).format("DD/MM/YYYY");
                ngModelCtrl.$setViewValue(date);
                //scope.$apply();
            });
        }
    };
});
app.directive('toUppercase', function () {

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            function parser(value) {
                if (ctrl.$isEmpty(value)) {
                    return value;
                }
                var formatedValue = value.toUpperCase();
                if (ctrl.$viewValue !== formatedValue) {
                    ctrl.$setViewValue(formatedValue);
                    ctrl.$render();
                }
                return formatedValue;
            }

            function formatter(value) {
                if (ctrl.$isEmpty(value)) {
                    return value;
                }
                return value.toUpperCase();
            }

            ctrl.$formatters.push(formatter);
            ctrl.$parsers.push(parser);
        }
    };
});

app.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9/]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

app.directive('currencyInput', function ($filter, $browser) {
    return {
        require: 'ngModel',
        link: function ($scope, $element, $attrs, ngModelCtrl) {
            var listener = function () {
                var value = $element.val().replace(/,/g, '')
                $element.val($filter('number')(value, false))
            }

            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function (viewValue) {
                return viewValue.replace(/,/g, '');
            })

            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function () {
                $element.val($filter('number')(ngModelCtrl.$viewValue, false))
            }

            $element.bind('change', listener)
            $element.bind('keydown', function (event) {
                var key = event.keyCode
                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40))
                    return
                $browser.defer(listener) // Have to do this or changes don't get picked up properly
            })

            $element.bind('paste cut', function () {
                $browser.defer(listener)
            })
        }

    }
});

app.directive('uiTreeView', [function () {
    return {
        require: "ngModel",
        restrict: "EAC",
        replace: true,
        scope: {
            treeViewInit: "="
        },
        link: function (scope, elem, attrs) {
            function initiateDemoData() {
                var tree_data = scope.treeViewInit.data;
                var dataSource1 = function (options, callback) {
                    var $data = null
                    if (!("text" in options) && !("type" in options)) {
                        $data = tree_data;
                        callback({ data: $data });
                        return;
                    }
                    else if ("type" in options && options.type == "folder") {
                        if ("additionalParameters" in options && "children" in options.additionalParameters)
                            $data = options.additionalParameters.children || {};
                        else $data = {}
                    }

                    if ($data != null)
                        setTimeout(function () { callback({ data: $data }); }, parseInt(Math.random() * 500) + 200);

                }
                return { 'dataSource1': dataSource1 }
            }
            var sampleData = initiateDemoData();
            $(elem[0]).ace_tree({
                dataSource: sampleData['dataSource1'],
                loadingHTML: '<div class="tree-loading"><i class="ace-icon fa fa-refresh fa-spin blue"></i></div>',
                'open-icon': 'ace-icon fa fa-folder-open',
                'close-icon': 'ace-icon fa fa-folder',
                'itemSelect': true,
                'folderSelect': true,
                'multiSelect': false,
                'selected-icon': null,
                'unselected-icon': null,
                'folder-open-icon': 'ace-icon tree-plus',
                'folder-close-icon': 'ace-icon tree-minus'
            });
        }
    };
}]);

app.filter('sumByColumn', function () {
    return function (collection, column) {
        var total = 0;

        collection.forEach(function (item) {
            total += parseInt(item[column]);
        });

        return total;
    };
});

app.filter("mydate", function () {
    var re = /\/Date\(([0-9]*)\)\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

app.directive('myEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.myEnter);
                });

                event.preventDefault();
            }
        });
    };
});
app.directive('compileTemplate', function ($compile, $parse) {
    return {
        link: function (scope, element, attr) {
            var parsed = $parse(attr.ngBindHtml);
            function getStringValue() { return (parsed(scope) || '').toString(); }

            //Recompile if the template changes
            scope.$watch(getStringValue, function () {
                $compile(element, null, -9999)(scope);  //The -9999 makes it skip directives so that we do not recompile ourselves
            });
        }
    }
});