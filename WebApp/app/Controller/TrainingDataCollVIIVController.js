app.controller("TrainingDataCollVIIVController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "record_id";

    $scope.ListDuAn = [];
    var dataTableTrainingDataCollVIIV = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {

        $scope.ListDuAn = [];
        GetBottomAction();
        $scope.LoadPage(1);
    });
    var d = new Date();
    var curr_datetime = d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + '-' + d.getHours() + d.getMinutes();

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/TrainingDataCollVIIV/GetBottomAction',
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
                if (response?.DuAns != null && response?.DuAns.length > 0) {
                    $scope.ListDuAn = response?.DuAns;
                }
                $scope.$apply();
            }
        });
    }

    $('#dataTableTrainingDataCollVIIV').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        
        $scope.ListTrainingDataCollVIIV = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableTrainingDataCollVIIV.destroy();

            dataTableTrainingDataCollVIIV = $('#dataTableTrainingDataCollVIIV').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                //serverSide: true,
                ordering: false,
                searching: true,
                processing: true,
                
                columns: [
                    { "data": "maduan", searchBuilderType: "string" },
                    //{ "data": "tenduan", searchBuilderType: "string" },
                    { "data": "CityName", searchBuilderType: "string" },
                    //{ "data": "manhom_tbh", searchBuilderType: "string"},
                    //{ "data": "tennhom_tbh", searchBuilderType: "string"},
                    //{ "data": "makh", searchBuilderType: "string"},
                    //{ "data": "hoten", searchBuilderType: "string"},
                    //{ "data": "ngayxntext", searchBuilderType: "string" },
                    { "data": "ngaythtext", searchBuilderType: "date" },
                    { "data": "doituong", searchBuilderType: "string" },
                    { "data": "taphuan", searchBuilderType: "string" },                    
                    { "data": "khac", searchBuilderType: "string" },
                    { "data": "noidung", searchBuilderType: "string" },
                    { "data": "nhataitro", searchBuilderType: "string" },
                    { "data": "nvngo", searchBuilderType: "string" },
                    { "data": "nvtccd", searchBuilderType: "string" },
                    { "data": "cbcqnn", searchBuilderType: "string" },
                    { "data": "nvtv", searchBuilderType: "string" },
                    { "data": "cbyt", searchBuilderType: "string" },
                    { "data": "scdi", searchBuilderType: "string" },
                    { "data": "khac1", searchBuilderType: "string" },
                    { "data": "tochuc1", searchBuilderType: "string" },
                    { "data": "nhataitro_2", searchBuilderType: "string" },
                    { "data": "ngo", searchBuilderType: "string" },
                    { "data": "tochuc1", searchBuilderType: "string" },
                    { "data": "qlnn", searchBuilderType: "string" },
                    { "data": "ccdv", searchBuilderType: "string" },
                    { "data": "ttcn", searchBuilderType: "string" },
                    { "data": "bc", searchBuilderType: "string" },
                    { "data": "scdi1", searchBuilderType: "string" },
                    { "data": "scdi2", searchBuilderType: "string" },
                    { "data": "tinh", searchBuilderType: "string" },
                    { "data": "tinh_2", searchBuilderType: "string" }
                ],
                "language": {
                    "emptyTable": "Không có dữ liệu",
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
                                "gt": 'Tốt hơn',
                                "gte": 'Lớn hơn hoặc bằng',
                                "lt": 'Ít hơn',
                                "lte": 'Ít hơn hoặc bằng',
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
                        title: 'TrainingDataCollection_' + curr_datetime
                    }
                ]
                ,
                columnDefs: [{
                    searchBuilder: {
                        defaultCondition: "="
                    },
                    targets: [1]
                }]
            });
        } else {
            dataTableTrainingDataCollVIIV.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/TrainingDataCollVIIV/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }
   
});
