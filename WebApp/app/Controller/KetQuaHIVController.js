app.controller("KetQuaHIVController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqxnhiv_id";

    $scope.ListDuAn = [];
    var dataTableKetQuaHIV = null;
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
            url: '/KetQuaHIV/GetBottomAction',
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
                if (data.DuAns != null && data.DuAns.length > 0) {
                    $scope.ListDuAn = data.DuAns;
                }
                $scope.$apply();
            }
        });
    }

    $('#dataTableKetQuaHIV').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        
        $scope.ListKetQuaHIV = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableKetQuaHIV.destroy();
            dataTableKetQuaHIV = $('#dataTableKetQuaHIV').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                //serverSide: true,
                ordering: false,
                searching: true,
                processing: true,
                //ajax: function (data, callback, settings) {
                //    var dataUser = [];
                //    var totalItems = 0;
                //    var page = ((data.start / data.length) + 1);

                //    $scope.modelSearch.currentPage = page;
                //    $scope.modelSearch.pageSize = data.length;

                //    $.ajax({
                //        type: 'post',
                //        url: '/KetQuaHIV/GetAll',
                //        cache: false,
                //        async: false,
                //        data: $scope.modelSearch,
                //        success: function (respone) {
                //            totalItems = respone.totalItems;
                //            $scope.ListKetQuaHIV = respone.data;
                //            if (respone.data != null && respone.data.length > 0) {
                //                for (var i = 0; i < respone.data.length; i++) {
                //                    var tmp = {
                //                        kqxnhiv_id: respone.data[i].kqxnhiv_id,
                //                        khachhang_id: respone.data[i].khachhang_id,
                //                        ngayxn: respone.data[i].ngayxn,
                //                        ngayxn_date: respone.data[i].ngayxn_date,
                //                        ngayxn_month: respone.data[i].ngayxn_month,
                //                        ngayxn_year: respone.data[i].ngayxn_year,
                //                        ketqua: respone.data[i].ketqua,
                //                        dangdieutri_hiv: respone.data[i].dangdieutri_hiv,
                //                        manhom_tbh: respone.data[i].manhom_tbh,
                //                        city_code: respone.data[i].city_code,
                //                        hoten: respone.data[i].hoten,
                //                        makh: respone.data[i].makh,
                //                        CityName: respone.data[i].CityName,
                //                        ngayxntext: respone.data[i].ngayxntext,
                //                        tennhom_tbh: respone.data[i].tennhom_tbh
                //                    }
                //                    dataUser.push(tmp);
                //                }
                //            }
                //        }
                //    });

                //    setTimeout(function () {
                //        callback({
                //            draw: data.draw,
                //            data: dataUser,
                //            recordsTotal: totalItems,
                //            recordsFiltered: totalItems
                //        });
                //    }, 50);
                //},
                //rowId: 'khachhang_id',
                //select: {
                //    info: false
                //},
                columns: [
                    { "data": "maduan", searchBuilderType: "string" },
                    //{ "data": "tenduan", searchBuilderType: "string" },
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "kqxnhiv_id", searchBuilderType: "string" },
                    { "data": "manhom_tbh", searchBuilderType: "string"},
                    { "data": "tennhom_tbh", searchBuilderType: "string"},
                    { "data": "makh", searchBuilderType: "string"},
                    //{ "data": "hoten", searchBuilderType: "string"},
                    //{ "data": "ngayxntext", searchBuilderType: "string" },
                    { "data": "ngayxn", searchBuilderType: "date" },
                    { "data": "ketqua", searchBuilderType: "string" },
                    { "data": "dangdieutri_hiv", searchBuilderType: "string" },
                    { "data": "lydo", searchBuilderType: "string" },
                    { "data": "tinhtrang", searchBuilderType: "string" },
                    { "data": "arv", searchBuilderType: "string" },
                    { "data": "cs_dieutri", searchBuilderType: "string" }
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
                        title: 'Xuất excel'
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
            dataTableKetQuaHIV.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/KetQuaHIV/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }
   
});
