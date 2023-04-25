app.controller("KetQuaBHDGController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "record_id";

    $scope.ListDuAn = [];
    var dataTableKetQuaBHDG = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {

        $scope.ListDuAn = [];
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/KetQuaBHDG/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnExportExcel') {
                            $scope.RoleBtnExportExcel = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                    });
                }
                if (response.DuAns != null && response.DuAns.length > 0) {
                    $scope.ListDuAn = response.DuAns;
                }
                $scope.$apply();
            }
        });
    }

    $('#dataTableKetQuaBHDG').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        
        $scope.ListKetQuaBHDG = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableKetQuaBHDG.destroy();
            dataTableKetQuaBHDG = $('#dataTableKetQuaBHDG').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                //serverSide: true,
                ordering: false,
                searching: true,
                processing: true,
                
                columns: [
                    { "data": "maduan", searchBuilderType: "string" },
                    //{ "data": "tenduan", searchBuilderType: "string" },
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "record_id_api", searchBuilderType: "string" },
                    { "data": "manhom_tbh", searchBuilderType: "string"},
                    { "data": "tennhom_tbh", searchBuilderType: "string"},
                    { "data": "makh", searchBuilderType: "string"},
                    //{ "data": "hoten", searchBuilderType: "string"},
                    //{ "data": "ngayxntext", searchBuilderType: "string" },
                    { "data": "ngaysl", searchBuilderType: "date" },
                    { "data": "cau1", searchBuilderType: "string" },
                    { "data": "cau2", searchBuilderType: "string" },
                    { "data": "cau3", searchBuilderType: "string" },
                    { "data": "cau4", searchBuilderType: "string" },
                    { "data": "cau5", searchBuilderType: "string" },
                    { "data": "cau6", searchBuilderType: "string" },
                    { "data": "cau7", searchBuilderType: "string" },
                    { "data": "cau8", searchBuilderType: "string" },
                    { "data": "cau9", searchBuilderType: "string" },
                    { "data": "cau10", searchBuilderType: "string" },
                    { "data": "cau11", searchBuilderType: "string" },
                    { "data": "cau12", searchBuilderType: "string" },
                    { "data": "cau13", searchBuilderType: "string" },
                    { "data": "cau14", searchBuilderType: "string" },
                    { "data": "cau15", searchBuilderType: "string" },
                    { "data": "cau16", searchBuilderType: "string" },
                    { "data": "cau17", searchBuilderType: "string" },
                    { "data": "cau18", searchBuilderType: "string" },
                    { "data": "cau19", searchBuilderType: "string" },
                    { "data": "cau20", searchBuilderType: "string" },
                    { "data": "cau21", searchBuilderType: "string" },
                    { "data": "cau22", searchBuilderType: "string" },
                    { "data": "cau23", searchBuilderType: "string" },
                    { "data": "cau24", searchBuilderType: "string" },
                    { "data": "cau25", searchBuilderType: "string" },
                    { "data": "cau26", searchBuilderType: "string" },
                    { "data": "cau27", searchBuilderType: "string" },                   
                ],
                "language": {
                    "emptyTable": "Không có dữ liệu trong bản",
                    "info": "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                    "infoEmpty": "Hiển thị 0 đến 0 của 0 bản ghi",
                    "infoFiltered": "(lọc từ _MAX_ tổng bản ghi)",
                    "lengthMenu": "Hiển thị _MENU_ bản ghi",
                    "loadingRecords": "Đang tải...",
                    "search": "Tìm kiếm:",
                    "zeroRecords": "Không tìm thấy kết quả",
                    "paginate": {
                        "first": "<<",
                        "last": ">>",
                        "next": ">",
                        "previous": "<"
                    },
                    "searchBuilder": {                        
                        "button": 'Tìm kiếm',
                        "add": 'Thêm điều kiện',
                        "clearAll": 'Xóa tất cả',
                        "condition": 'Điều kiện',
                        "conditions": {
                            "date": {
                                "before": 'Trước',
                                "after": 'Sau',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "moment": {
                                "before": 'Trước',
                                "after": 'Sau',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "number": {
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "gt": 'Lớn hơn',
                                "gte": 'Lớn hơn hoặc bằng',
                                "lt": 'Nhỏ hơn',
                                "lte": 'Nhỏ hơn hoặc bằng',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "string": {
                                "contains": 'Chứa',
                                "notContains": 'Không chứa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "endsWith": 'Kết thúc bằng',
                                "startsWith": 'Bắt đầu bằng',
                                "notEndsWith": 'Không kết thúc bằng',
                                "notStartsWith": 'Không bắt đầu bằng'
                            },
                        },
                        "data": 'Cột',
                        "logicAnd": 'Và',
                        "logicOr": 'Hoặc',
                        "title": {
                            0: 'Các điều kiện tìm kiếm',
                            _: 'Đã lọc (%d)'
                        },
                        "deleteTitle": 'Xóa',
                        "leftTitle": 'Bên trái',
                        "rightTitle": 'Đúng',
                        "value": 'Giá trị',
                    }
                },
                dom:
                    "<'row'<'col-sm-8'Q>>" +
                    "<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",
                scroller: {
                    loadingIndicator: true
                },
                buttons: [
                    {
                        extend: 'excelHtml5',
                        title: null,
                        sheetName: 'KetQuaBHDG_' + curr_datetime
                    }
                ]
                ,
                preDefined: {
                    criteria: [
                        {
                            data: 'cau1',
                            condition: '=',
                            value: ['Sai']
                        }
                    ]
                },
                columnDefs: [{
                    searchBuilder: {
                        defaultCondition: "="
                    },
                    targets: [1]
                }]
            });
        } else {
            dataTableKetQuaBHDG.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/KetQuaBHDG/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }
   
});
