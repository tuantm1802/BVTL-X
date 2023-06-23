app.controller("PhieuTuVanController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "record_id_api";
    $scope.ListDuAn = [];
    var dataTablePhieuTuVan = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.ListDuAn = [];
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;
    var d = new Date();
    var curr_datetime = d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + '-' + d.getHours() + d.getMinutes();

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/PhieuTuVan/GetBottomAction',
            data: {},
            success: function (response) {
                console.log(response);
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

    $('#dataTablePhieuTuVan').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();

        $scope.ListPhieuTuVan = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTablePhieuTuVan.destroy();
            dataTablePhieuTuVan = $('#dataTablePhieuTuVan').DataTable({
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
                //        url: '/PhieuTuVan/GetAll',
                //        cache: false,
                //        async: false,
                //        data: $scope.modelSearch,
                //        success: function (respone) {
                //            totalItems = respone.totalItems;
                //            $scope.ListPhieuTuVan = respone.data;
                //            if (respone.data != null && respone.data.length > 0) {
                //                for (var i = 0; i < respone.data.length; i++) {
                //                    var tmp = {
                //                        kqslace_id: respone.data[i].kqslace_id,
                //                        khachhang_id: respone.data[i].khachhang_id,
                //                        ngaysl: respone.data[i].ngaysl,
                //                        ngaysl_date: respone.data[i].ngaysl_date,
                //                        ngaysl_month: respone.data[i].ngaysl_month,
                //                        ngaysl_year: respone.data[i].ngaysl_year,
                //                        tongdiem_ace: respone.data[i].tongdiem_ace,
                //                        ketqua_ace: respone.data[i].ketqua_ace,
                //                        manhom_tbh: respone.data[i].manhom_tbh,
                //                        city_code: respone.data[i].city_code,
                //                        hoten: respone.data[i].hoten,
                //                        makh: respone.data[i].makh,
                //                        CityName: respone.data[i].CityName,
                //                        ngaysltext: respone.data[i].ngaysltext,
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
                //rowId: 'kqslace_id',
                //select: {
                //    info: false
                //},
                columns: [
                    { "data": "maduan", searchBuilderType: "string" },
                    //{ "data": "tenduan", searchBuilderType: "string" },
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "record_id_api", searchBuilderType: "string" },
                    { "data": "manhom_tbh", searchBuilderType: "string"},
                    { "data": "tennhom_tbh", searchBuilderType: "string"},
                    { "data": "makh", searchBuilderType: "string"},
                    //{ "data": "hoten", searchBuilderType: "string"},
                    { "data": "ngaynhap", searchBuilderType: "date" },
                    { "data": "ngaytuvan", searchBuilderType: "date" },
                    { "data": "diadiem", searchBuilderType: "string" },
                    { "data": "matcv", searchBuilderType: "string" },
                    { "data": "lantuvan", searchBuilderType: "string" },
                    { "data": "cau1_1", searchBuilderType: "string" },
                    { "data": "cau1_1k", searchBuilderType: "string" },
                    { "data": "cau1_2", searchBuilderType: "string" },
                    { "data": "cau1_3", searchBuilderType: "string" },
                    { "data": "cau1_4", searchBuilderType: "string" },
                    { "data": "cau1_5", searchBuilderType: "string" },
                    { "data": "cau2", searchBuilderType: "string" },
                    { "data": "cau2_1k", searchBuilderType: "string" },
                    { "data": "cau2_2", searchBuilderType: "string" },
                    { "data": "cau3", searchBuilderType: "string" },
                    { "data": "cau3_1k", searchBuilderType: "string" },
                    { "data": "cau3_1k_2", searchBuilderType: "string" },
                    { "data": "cau4", searchBuilderType: "string" },
                    { "data": "cau4_1k", searchBuilderType: "string" },
                    { "data": "cau4_1k_2", searchBuilderType: "string" },
                    { "data": "cau5_1", searchBuilderType: "string" },
                    { "data": "cau5_1k", searchBuilderType: "string" },
                    { "data": "cau5_1_2", searchBuilderType: "string" },
                    { "data": "cau5_2", searchBuilderType: "string" },
                    { "data": "cau5_2k", searchBuilderType: "string" },
                    { "data": "cau5_2k_2", searchBuilderType: "string" },
                    { "data": "cau5_3", searchBuilderType: "string" },
                    { "data": "cau5_3_1", searchBuilderType: "string" },
                    { "data": "cau5_4", searchBuilderType: "string" },
                    { "data": "cau5_4_1", searchBuilderType: "string" },
                    { "data": "cau5_5", searchBuilderType: "string" },
                    { "data": "cau5_4_2", searchBuilderType: "string" },
                    { "data": "cau5_6", searchBuilderType: "string" },
                    { "data": "cau5_6k", searchBuilderType: "string" },
                    { "data": "cau5_6k_2", searchBuilderType: "string" },
                    { "data": "cau5_7", searchBuilderType: "string" },
                    { "data": "cau5_7k", searchBuilderType: "string" },
                    { "data": "cau5_7k_3", searchBuilderType: "string" },
                    { "data": "cau5_8", searchBuilderType: "string" },
                    { "data": "cau5_7k_2", searchBuilderType: "string" },
                    { "data": "tongket", searchBuilderType: "string" },
                    { "data": "tuvantiep", searchBuilderType: "string" },
                    { "data": "vande", searchBuilderType: "string" },
                    { "data": "thoigian", searchBuilderType: "date" },
                    

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
                        title: null,
                        sheetName: 'PhieuTuVan_' + curr_datetime
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
            dataTablePhieuTuVan.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/PhieuTuVan/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});